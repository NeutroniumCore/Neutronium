using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MVVM.HTML.Core;
using MVVM.HTML.Core.Infra.VM;
using MVVM.HTML.Core.JavascriptEngine;
using MVVM.HTML.Core.Navigation;
using MVVM.HTML.Core.Window;
using System.Diagnostics;
using MVVM.HTML.Core.Exceptions;

namespace HTML_WPF.Component
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class HTMLControlBase : UserControl, IWebViewLifeCycleManager, IDisposable
    {
        private IWPFWebWindowFactory _IWPFWebWindowFactory;
        private IWebSessionWatcher _IWebSessionWatcher = new NullWatcher();
        private IUrlSolver _IUrlSolver;
        private WPFDoubleBrowserNavigator _WPFDoubleBrowserNavigator;
        private string _KoView = null;

        public Boolean IsDebug
        {
            get { return (Boolean)this.GetValue(IsDebugProperty); }
            set { this.SetValue(IsDebugProperty, value); }
        }

        public static readonly DependencyProperty IsDebugProperty =
            DependencyProperty.Register("IsDebug", typeof(Boolean), typeof(HTMLControlBase), new PropertyMetadata(false));


        public Boolean IsHTMLLoaded
        {
            get { return (Boolean)this.GetValue(IsHTMLLoadedProperty); }
            private set { this.SetValue(IsHTMLLoadedProperty, value); }
        }

        public static readonly DependencyProperty IsHTMLLoadedProperty =
            DependencyProperty.Register("IsHTMLLoaded", typeof(Boolean), typeof(HTMLControlBase), new PropertyMetadata(false));

        public string HTMLEngine
        {
            get { return (string)this.GetValue(HTMLEngineProperty); }
            set { this.SetValue(HTMLEngineProperty, value); }
        }

        public static readonly DependencyProperty HTMLEngineProperty =
            DependencyProperty.Register("HTMLEngine", typeof(string), typeof(HTMLControlBase), new PropertyMetadata(string.Empty));


        public string Source
        {
            get { return _WPFDoubleBrowserNavigator.Url; }
        }

        public bool UseINavigable
        {
            get { return _WPFDoubleBrowserNavigator.UseINavigable; }
            set { _WPFDoubleBrowserNavigator.UseINavigable = value; }
        }

        public ICommand DebugWindow { get; private set; }

        public ICommand DebugBrowser { get; private set; }   

        protected HTMLControlBase(IUrlSolver iIUrlSolver)
        {
            _IUrlSolver = iIUrlSolver;

            DebugWindow = new BasicRelayCommand(() => ShowDebugWindow());
            DebugBrowser = new BasicRelayCommand(() => OpenDebugBrowser());

            InitializeComponent();

            _IWPFWebWindowFactory = HTMLEngineFactory.Engine.Resolve(HTMLEngine);

            _WPFDoubleBrowserNavigator = new WPFDoubleBrowserNavigator(this, _IUrlSolver);
            _WPFDoubleBrowserNavigator.OnFirstLoad += FirstLoad;
        }

        public IWebSessionWatcher WebSessionWatcher
        {
            get { return _IWebSessionWatcher; }
            set { _IWebSessionWatcher = value; _WPFDoubleBrowserNavigator.WebSessionWatcher = value; }
        }

        private void FirstLoad(object sender, EventArgs e)
        {
            IsHTMLLoaded = true;
            _WPFDoubleBrowserNavigator.OnFirstLoad -= FirstLoad;
        }
       
        private void RunKoView()
        {
            if (_KoView == null)
            {
                using (Stream stream = typeof(IHTMLBinding).Assembly.
                        GetManifestResourceStream("MVVM.HTML.Core.Navigation.javascript.ko-view.min.js"))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        _KoView = reader.ReadToEnd();
                    }
                }
            }
            _WPFDoubleBrowserNavigator.ExcecuteJavascript(_KoView);
        }

        public void ShowDebugWindow()
        {
            RunKoView();
            _WPFDoubleBrowserNavigator.ExcecuteJavascript("ko.dodebug();");
        }

        public void OpenDebugBrowser()
        {
            Nullable<int> RemoteDebuggingPort = _IWPFWebWindowFactory.GetRemoteDebuggingPort();
            if (RemoteDebuggingPort!=null)
                Process.Start(string.Format("http://localhost:{0}/", RemoteDebuggingPort));
            else
                MessageBox.Show("EnableBrowserDebug should be set to true to enable debugging in a Webrowser!");
        }

        protected async Task NavigateAsyncBase(object iViewModel, string Id = null, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay)
        {
            await _WPFDoubleBrowserNavigator.NavigateAsync(iViewModel, Id, iMode);
        }

        public void Dispose()
        {
            _WPFDoubleBrowserNavigator.Dispose();
        }

        public event EventHandler<NavigationEvent> OnNavigate
        {
            add { _WPFDoubleBrowserNavigator.OnNavigate += value; }
            remove { _WPFDoubleBrowserNavigator.OnNavigate -= value; }
        }

        public event EventHandler OnFirstLoad
        {
            add { _WPFDoubleBrowserNavigator.OnFirstLoad += value; }
            remove { _WPFDoubleBrowserNavigator.OnFirstLoad -= value; }
        }

        public event EventHandler<DisplayEvent> OnDisplay
        {
            add { _WPFDoubleBrowserNavigator.OnDisplay += value; }
            remove { _WPFDoubleBrowserNavigator.OnDisplay -= value; }
        }

        private void Root_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5) e.Handled = true;
        }

        private string _WebSessionPath = null;
        public string SessionPath
        {
            get { return _WebSessionPath; }
            set 
            { 
                _WebSessionPath = value;
                if (_WebSessionPath != null)
                    Directory.CreateDirectory(_WebSessionPath);
            }
        }

        IHTMLWindowProvider IWebViewLifeCycleManager.Create()
        {
            if (_IWPFWebWindowFactory == null)
            {
                _IWPFWebWindowFactory = HTMLEngineFactory.Engine.Resolve(HTMLEngine);

                if (_IWPFWebWindowFactory==null)
                    throw ExceptionHelper.Get(string.Format("Not able to find WebEngine {0}", HTMLEngine));
            }

            IWPFWebWindow webwindow = _IWPFWebWindowFactory.Create();
            
            var ui = webwindow.UIElement;
            Grid.SetColumnSpan(ui, 2);
            Grid.SetRowSpan(ui, 2);
            Panel.SetZIndex(ui, 0);
            this.MainGrid.Children.Add(ui);
            return new WPFHTMLWindowProvider(webwindow, this );
        }

        IDispatcher IWebViewLifeCycleManager.GetDisplayDispatcher()
        {
            return new WPFUIDispatcher(this.Dispatcher);
        }

        public void Inject(Key KeyToInject)
        {
            var wpfacess =  (_WPFDoubleBrowserNavigator.WebControl as WPFHTMLWindowProvider);
            if (wpfacess != null)
                return;

            var wpfweb = wpfacess.IWPFWebWindow;            
            if (wpfweb!=null)
                wpfweb.Inject(KeyToInject);
        }

        //private void WebCore_ShuttingDown(object sender, CoreShutdownEventArgs e)
        //{
        //    //It is possible that webcore is shutting because the window is closing
        //    //In this case I don't have to raise a session error
        //    if (Application.Current == null)
        //    {
        //        e.Cancel = true;
        //        return;
        //    }

        //    _IWebSessionWatcher.LogCritical("Critical: WebCore ShuttingDown!!");

        //    Trace.WriteLine(string.Format("MVVM for awesomium: WebCoreShutting Down exception: {0}", e.Exception));

        //    _IWebSessionWatcher.OnSessionError(e.Exception, () => e.Cancel = true);
        //}

        //void IWebViewLifeCycleManager.Display(WpfCefBrowser webview)
        //{
        //    webview.Visibility = Visibility.Visible;
        //}

        //void IWebViewLifeCycleManager.Dispose(WpfCefBrowser ioldwebview)
        //{
        //    //var wb = (ioldwebview as WebControl);
        //    ioldwebview.Visibility = Visibility.Hidden;

        //    //if (!ioldwebview.IsCrashed)
        //    //{
        //    //    ioldwebview.Source = new Uri("about:blank");
        //    //}

        //    //WebCore.ShuttingDown -= WebCore_ShuttingDown;
        //    this.MainGrid.Children.Remove(ioldwebview);

        //    ioldwebview.Dispose();

        //}
    }
}
