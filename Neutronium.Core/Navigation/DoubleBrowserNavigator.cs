using System;
using System.Threading.Tasks;
using Neutronium.Core.Binding;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.Navigation.Window;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.Core.Log;

namespace Neutronium.Core.Navigation
{
    public class DoubleBrowserNavigator : INavigationSolver 
    {
        private readonly IWebViewLifeCycleManager _WebViewLifeCycleManager;
        private readonly IJavascriptFrameworkManager _JavascriptFrameworkManager;
        private readonly IUrlSolver _UrlSolver;        
        private IWebBrowserWindowProvider _CurrentWebControl;
        private IWebBrowserWindowProvider _NextWebControl;
        private IHTMLBinding _HTMLBinding;
        private IWebSessionLogger _webSessionLogger;
        private bool _Disposed = false;
        private bool _Navigating = false;
        private bool _UseINavigable = false;
        private HTMLLogicWindow _Window;

        public Uri Url { get; private set; }
        public IWebBrowserWindowProvider WebControl => _CurrentWebControl;
        public IWebBrowserWindow HTMLWindow => _CurrentWebControl?.HTMLWindow;
        public bool UseINavigable 
        {
            get { return _UseINavigable; }
            set { _UseINavigable = value; }
        }

        public IWebSessionLogger WebSessionLogger
        {
            set { _webSessionLogger = value; }
        }

        public DoubleBrowserNavigator(IWebViewLifeCycleManager lifecycler, IUrlSolver urlSolver, IJavascriptFrameworkManager javascriptFrameworkManager)
        {
            _webSessionLogger = new BasicLogger();
            _JavascriptFrameworkManager = javascriptFrameworkManager;
            _WebViewLifeCycleManager = lifecycler;
            _UrlSolver = urlSolver;
        }

        private void ConsoleMessage(object sender, ConsoleMessageArgs e)
        { 
            try
            {
                _webSessionLogger.LogBrowser(e, Url);
            }
            catch { }
        }

        public IHTMLBinding Binding
        {
            get { return _HTMLBinding; }
            private set
            {
                _HTMLBinding?.Dispose();

                _HTMLBinding = value;

                if (_Disposed && (_HTMLBinding!=null))
                   Binding = null;
            }
        }

        private void FireNavigate(object newVm, object oldVm=null) 
        {
            OnNavigate?.Invoke(this, new NavigationEvent(newVm, oldVm));
        }
    
        private void FireLoaded(object loadedVm)
        {
            OnDisplay?.Invoke(this, new DisplayEvent(loadedVm));
        }

        private void Switch(Task<IHTMLBinding> binding, HTMLLogicWindow window, TaskCompletionSource<IHTMLBinding> tcs)
        {
            var oldvm = Binding?.Root;
            var fireFirstLoad = false;
            Binding = binding.Result;
          
            if (_CurrentWebControl!=null)
            {
                _CurrentWebControl.HTMLWindow.ConsoleMessage -= ConsoleMessage;
                _CurrentWebControl.Dispose();
            }
            else 
            {
                fireFirstLoad = true;
            }

            _CurrentWebControl = _NextWebControl;     
            _NextWebControl = null;
            _CurrentWebControl.HTMLWindow.Crashed += Crashed;

            _CurrentWebControl.Show();
    
            _Window = window; 

            var inav = _UseINavigable ? Binding.Root as INavigable : null;
            if (inav != null)
                inav.Navigation = this;
            _Window.State = WindowLogicalState.Opened;

            _Window.OpenAsync().ContinueWith(t => EndAnimation(Binding.Root));

            _Navigating = false;

            FireNavigate(Binding.Root, oldvm);

            tcs?.SetResult(Binding);

            if (fireFirstLoad)
                OnFirstLoad?.Invoke(this, EventArgs.Empty);        
        }    

        private void EndAnimation(object navigable)
        {
            _WebViewLifeCycleManager.GetDisplayDispatcher()
                .RunAsync( () => FireLoaded(navigable) );
        }

        private void Crashed(object sender, BrowserCrashedArgs e)
        {
            var dest = _CurrentWebControl.HTMLWindow.Url;
            var vm = Binding.Root;
            var mode = Binding.Mode;

            _webSessionLogger.Error("WebView crashed trying recover");

            CleanWebControl(ref _CurrentWebControl);
            Binding = null;

            Navigate(dest, vm, mode);
        }

        private Task<IHTMLBinding> Navigate(Uri uri, object viewModel, JavascriptBindingMode mode = JavascriptBindingMode.TwoWay)
        {
            if (uri == null)
                throw ExceptionHelper.GetArgument($"ViewModel not registered: {viewModel.GetType()}");

            _Navigating = true;

            var oldvm = Binding?.Root as INavigable;

            if (_UseINavigable && (oldvm!=null))
            {
                oldvm.Navigation = null;
            }

            var wh = new WindowHelper(new HTMLLogicWindow());

            if (_CurrentWebControl != null)
                _CurrentWebControl.HTMLWindow.Crashed -= Crashed;

            var closetask = ( _CurrentWebControl!=null) ? _Window.CloseAsync() : TaskHelper.Ended();

            _NextWebControl = _WebViewLifeCycleManager.Create();
            _NextWebControl.HTMLWindow.ConsoleMessage += ConsoleMessage;

            var moderWindow = _NextWebControl.HTMLWindow as IModernWebBrowserWindow;

            var injectorFactory = GetInjectorFactory(uri);
            var engine = new HTMLViewEngine(_NextWebControl, injectorFactory, _webSessionLogger);
            var initVm = HTML_Binding.GetBindingBuilder(engine, viewModel, mode, wh);

            if (moderWindow!=null)
            {
                var debugContext = _WebViewLifeCycleManager.DebugContext;
                EventHandler<BeforeJavascriptExcecutionArgs> before = null;
                before = (o,e) =>
                {
                    moderWindow.BeforeJavascriptExecuted -= before;
                    e.JavascriptExecutor(_JavascriptFrameworkManager.GetMainScript(debugContext));
                };
                moderWindow.BeforeJavascriptExecuted += before;
            }
            var tcs = new TaskCompletionSource<IHTMLBinding>();

            EventHandler<LoadEndEventArgs> sourceupdate = null;
            sourceupdate = async (o, e) =>
            {
                _NextWebControl.HTMLWindow.LoadEnd -= sourceupdate;

                var builder = await initVm;
                await builder.CreateBinding(_WebViewLifeCycleManager.DebugContext).WaitWith(closetask, t => Switch(t, wh.__window__, tcs)).ConfigureAwait(false);
            };

            Url = uri;
            _NextWebControl.HTMLWindow.LoadEnd += sourceupdate;
            _NextWebControl.HTMLWindow.NavigateTo(uri);

            return tcs.Task;
        }

        private IJavascriptFrameworkManager GetInjectorFactory(Uri uri)
        {
            return _JavascriptFrameworkManager;
        }

        public void ExcecuteJavascript(string code)
        {
            try 
            {
                _CurrentWebControl?.HTMLWindow.MainFrame.ExecuteJavaScript(code);
            }
            catch(Exception e)
            {
                _webSessionLogger.Error($"Can not execute javascript: {code}, reason: {e}");
            }          
        }

        public async Task<IHTMLBinding> NavigateAsync(object viewModel, string id = "", JavascriptBindingMode mode = JavascriptBindingMode.TwoWay)
        {
            if ((viewModel == null) || (_Navigating))
                return null;

            var viewPath = _UrlSolver.Solve(viewModel, id);
            if (viewPath == null)
                throw ExceptionHelper.Get($"Unable to locate ViewModel {viewModel}");

            return await Navigate(viewPath, viewModel, mode);
        }

        public void Dispose()
        {
            _Disposed = true;
            Binding = null;
            UseINavigable = false;

            CleanWebControl(ref _CurrentWebControl);
            CleanWebControl(ref _NextWebControl);
        }

        private void CleanWebControl(ref IWebBrowserWindowProvider WebControl)
        {
            if (WebControl == null)
                return;

            WebControl.HTMLWindow.Crashed -= Crashed;
            WebControl.HTMLWindow.ConsoleMessage -= ConsoleMessage;
            WebControl.Dispose();
            WebControl = null;
        }

        public event EventHandler<NavigationEvent> OnNavigate;

        public event EventHandler<DisplayEvent> OnDisplay;

        public event EventHandler OnFirstLoad;
    }
}
