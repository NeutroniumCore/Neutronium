using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.IO;
using System.Windows;
using System.Diagnostics;

using Xilium.CefGlue;
using Xilium.CefGlue.WPF;

using MVVM.CEFGlue.Navigation.Window;
using MVVM.CEFGlue.Infra;
using MVVM.CEFGlue.Exceptions;
using MVVM.CEFGlue.Navigation;
using CefGlue.Window;

namespace MVVM.CEFGlue
{
    public class WPFDoubleBrowserNavigator : INavigationSolver
        //, IWebViewProvider
    {
        private IWebViewLifeCycleManager _IWebViewLifeCycleManager;

        private WpfCefBrowser _CurrentWebControl;
        private WpfCefBrowser _NextWebControl;


        private IHTMLBindingFactory _IAwesomiumBindingFactory;
        private IHTMLBinding _IAwesomeBinding;
        private IUrlSolver _INavigationBuilder;
        private bool _Disposed = false;
        private HTMLLogicWindow _Window;
        private bool _Navigating = false;

        internal WpfCefBrowser WebControl { get { return _CurrentWebControl; } }

        private IWebSessionWatcher _IWebSessionWatcher = new NullWatcher();

        public IWebSessionWatcher WebSessionWatcher
        {
            get { return _IWebSessionWatcher; }
            set { _IWebSessionWatcher = value; }
        }

        public WPFDoubleBrowserNavigator(IWebViewLifeCycleManager lifecycler, IUrlSolver inb, IHTMLBindingFactory iAwesomiumBindingFactory = null)
        {
            _IWebViewLifeCycleManager = lifecycler;
            _INavigationBuilder = inb;
            _IAwesomiumBindingFactory = iAwesomiumBindingFactory ?? new HTMLBindingFactory() { ManageWebSession = false };
        }

        private void ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        { 
            try
            {
                LogBrowser(string.Format("{0}, source {1}, line number {2}, page {3}", e.Message, e.Source, e.Line, (_CurrentWebControl==null) ? null : _CurrentWebControl.Url));
            }
            catch { }
        }

        private IHTMLBinding Binding
        {
            get { return _IAwesomeBinding; }
            set
            {
                if (_IAwesomeBinding != null)
                    _IAwesomeBinding.Dispose();

                _IAwesomeBinding = value;
                if (_Disposed && (_IAwesomeBinding!=null))
                {
                    _IAwesomeBinding.Dispose();
                    _IAwesomeBinding = null;
                }
            }
        }

        private void FireNavigate(object inewvm, object ioldvm=null) 
        {
            if (OnNavigate != null)
                OnNavigate(this, new NavigationEvent(inewvm, ioldvm));
        }

        private void FireLoaded(object iloadedvm)
        {
            if (OnDisplay != null)
                OnDisplay(this, new DisplayEvent(iloadedvm));
        }

        private void Switch(Task<IHTMLBinding> iBinding, HTMLLogicWindow iwindow, TaskCompletionSource<IHTMLBinding> tcs)
        {
            object oldvm = (Binding != null) ? Binding.Root : null;
            Binding = iBinding.Result;
          
            if (_CurrentWebControl!=null)
            {
                _CurrentWebControl.ConsoleMessage -= ConsoleMessage;
                _IWebViewLifeCycleManager.Dispose(_CurrentWebControl);
            }
            else if (OnFirstLoad != null)
                OnFirstLoad(this, EventArgs.Empty);

            _CurrentWebControl = _NextWebControl;     
            _NextWebControl = null;
            //_CurrentWebControl.Crashed += Crashed;

            _IWebViewLifeCycleManager.Display(_CurrentWebControl);
    
            _Window = iwindow; 

            _Window.State = WindowLogicalState.Opened;

            _Window.OpenAsync().ContinueWith(t => EndAnimation(Binding.Root));

            _Navigating = false;
            var inav = _UseINavigable ? Binding.Root as INavigable : null;
            if (inav != null)
                inav.Navigation = this;

            FireNavigate(Binding.Root, oldvm);
            
            if (tcs != null) tcs.SetResult(Binding);
        }

       

        private void EndAnimation(object inavgable)
        {
            Action action = () => FireLoaded(inavgable);
            this._CurrentWebControl.Dispatcher.BeginInvoke(action);
        }

        private void LogCritical(string iMessage)
        {
            _IWebSessionWatcher.LogCritical(iMessage);

            Trace.WriteLine(string.Format("MVVM for CEFGlue: Critical: {0}", iMessage));
        }

        private void LogBrowser(string iMessage)
        {
            _IWebSessionWatcher.LogBrowser(iMessage);

            Trace.WriteLine(string.Format("MVVM for CEFGlue: WebSession log message: {0}", iMessage));
        }

        //private void Crashed(object sender, CrashedEventArgs e)
        //{
        //    if ((WebCore.IsShuttingDown) || (!WebCore.IsInitialized) || (Application.Current==null))
        //        return;

        //    var dest = _CurrentWebControl.Source;
        //    var vm = Binding.Root;

        //    LogCritical("WebView crashed trying recover");
   
        //    _IWebViewLifeCycleManager.Dispose(_CurrentWebControl);
        //    _CurrentWebControl.ConsoleMessage -= ConsoleMessage;
        //    _CurrentWebControl.Crashed -= Crashed;
        //    _CurrentWebControl = null;

        //    Binding = null;

        //    WebCore.QueueWork(() => Navigate(dest, vm, JavascriptBindingMode.TwoWay));
        //}

        public Task Navigate(string iUri, object iViewModel, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay)
        {
            if (iUri == null)
                throw ExceptionHelper.GetArgument(string.Format("ViewModel not registered: {0}", iViewModel.GetType()));

            _Navigating = true;

            INavigable oldvm = (Binding != null) ? Binding.Root as INavigable : null;

            if (_UseINavigable && (oldvm!=null))
            {
                oldvm.Navigation = null;
            }

            var wh = new WindowHelper(new HTMLLogicWindow());

            //if (_CurrentWebControl != null)
            //    _CurrentWebControl.Crashed -= Crashed;

            Task closetask = ( _CurrentWebControl!=null) ? _Window.CloseAsync() : TaskHelper.Ended();

            _NextWebControl = _IWebViewLifeCycleManager.Create();
            _NextWebControl.ConsoleMessage += ConsoleMessage;

            TaskCompletionSource<IHTMLBinding> tcs = new TaskCompletionSource<IHTMLBinding>();

            EventHandler<LoadEndEventArgs> sourceupdate = null;
            sourceupdate = (o, e) =>
            {
                _NextWebControl.LoadEnd -= sourceupdate;
                _IAwesomiumBindingFactory.Bind(_NextWebControl, iViewModel, wh, iMode).WaitWith(closetask,
                     t => Switch(t, wh.__window__, tcs));
            };

            _NextWebControl.LoadEnd += sourceupdate;

            if (_NextWebControl.Url == iUri)
                _NextWebControl.Refresh();
            else
                _NextWebControl.NavigateTo(iUri);

            return tcs.Task;
        }

        public void ExcecuteJavascript(string icode)
        {
            try 
            {
                _CurrentWebControl.ExecuteJavaScript(icode, "WPFDoubleBrowserNavigator", 219);
            }
            catch(Exception e)
            {
                LogBrowser(string.Format("Can not execute javascript: {0}, reason: {1}",icode,e));
            }
            
        }

        public Task NavigateAsync(object iViewModel, string Id = null, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay)
        {
            if ((iViewModel == null) || (_Navigating))
                return TaskHelper.Ended();

            return Navigate(_INavigationBuilder.Solve(iViewModel, Id).LocalPath, iViewModel, iMode);
        }

        public void Dispose()
        {
            _Disposed = true;
            Binding = null;
            UseINavigable = false;

            CleanWebControl(ref _CurrentWebControl);
            CleanWebControl(ref _NextWebControl);
        }

        private void CleanWebControl(ref WpfCefBrowser iWebControl)
        {
            if (iWebControl == null)
                return;

            //iWebControl.Crashed -= Crashed;
            iWebControl.ConsoleMessage -= ConsoleMessage;
            _IWebViewLifeCycleManager.Dispose(iWebControl);
            iWebControl = null;
        }

        private bool _UseINavigable = false;
        public bool UseINavigable
        {
            get { return _UseINavigable; }
            set { _UseINavigable = value; }
        }

        //public IWebView WebView
        //{
        //    get { return this._CurrentWebControl; }
        //}

        public event EventHandler<NavigationEvent> OnNavigate;

        public event EventHandler<DisplayEvent> OnDisplay;

        public event EventHandler OnFirstLoad;
    }
}
