using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Diagnostics;
using MVVM.CEFGlue.Infra.VM;
using MVVM.CEFGlue.Navigation;
using Xilium.CefGlue;
using Xilium.CefGlue.WPF;
using MVVM.CEFGlue.CefSession;
using System.ComponentModel;

namespace MVVM.CEFGlue
{
    public partial class HTMLControlBase : UserControl, IWebViewLifeCycleManager, IDisposable
    {
        private static CefCoreSession _CefCoreSession;

        private IWebSessionWatcher _IWebSessionWatcher = new NullWatcher();

        public IWebSessionWatcher WebSessionWatcher
        {
            get { return _IWebSessionWatcher; }
            set { _IWebSessionWatcher = value; _WPFDoubleBrowserNavigator.WebSessionWatcher = value; }
        }

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


        public ICommand DebugWindow { get; private set; }

        public ICommand DebugBrowser { get; private set; }


        private IUrlSolver _IUrlSolver;

        private WPFDoubleBrowserNavigator _WPFDoubleBrowserNavigator;

        protected HTMLControlBase(IUrlSolver iIUrlSolver)
        {
            if ((_CefCoreSession==null) && !DesignerProperties.GetIsInDesignMode(this))
            {
                _CefCoreSession = CefCoreSessionSingleton.GetAndInitIfNeeded(new WPFUIDispatcher(this.Dispatcher));
            }
     
            _IUrlSolver = iIUrlSolver;
  
            DebugWindow = new BasicRelayCommand(() => ShowDebugWindow());

            DebugBrowser = new BasicRelayCommand(() => OpenDebugBrowser());

            InitializeComponent();
            _WPFDoubleBrowserNavigator = new WPFDoubleBrowserNavigator(this, _IUrlSolver);
            _WPFDoubleBrowserNavigator.OnFirstLoad += FirstLoad;

        }

        private void FirstLoad(object sender, EventArgs e)
        {
            IsHTMLLoaded = true;
            _WPFDoubleBrowserNavigator.OnFirstLoad -= FirstLoad;
        }

        private string _KoView = null;
        private void RunKoView()
        {
            if (_KoView == null)
            {
                using (Stream stream = Assembly.GetExecutingAssembly().
                        GetManifestResourceStream("MVVM.CEFGlue.Navigation.javascript.ko-view.min.js"))
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
            if (_CefCoreSession.CefSettings.RemoteDebuggingPort!=null)
                Process.Start(string.Format("http://localhost:{0}/", _CefCoreSession.CefSettings.RemoteDebuggingPort));
            else
                MessageBox.Show("EnableBrowserDebug should be set to true to enable debugging in a Webrowser!");
        }

        public string Source
        {
            get { return _WPFDoubleBrowserNavigator.WebControl.Url; }
        }

        public bool UseINavigable
        {
            get { return _WPFDoubleBrowserNavigator.UseINavigable; }
            set { _WPFDoubleBrowserNavigator.UseINavigable = value; }
        }

        protected Task NavigateAsyncBase(object iViewModel, string Id = null, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay)
        {
            return _WPFDoubleBrowserNavigator.NavigateAsync(iViewModel, Id, iMode);
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
            set { _WebSessionPath = value; }
        }

        WpfCefBrowser IWebViewLifeCycleManager.Create()
        {
            //if (_Session == null)
            //{
            //    _Session = (_WebSessionPath != null) ? WebCore.CreateWebSession(_WebSessionPath, new WebPreferences()) :
            //            WebCore.CreateWebSession(new WebPreferences());

            //    WebCore.ShuttingDown += WebCore_ShuttingDown;
            //}

            WpfCefBrowser nw = new WpfCefBrowser()
            {
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                ContextMenu = new ContextMenu() { Visibility = Visibility.Collapsed }
            };
            Grid.SetColumnSpan(nw, 2);
            Grid.SetRowSpan(nw, 2);
            Panel.SetZIndex(nw, 0);
            this.MainGrid.Children.Add(nw);
            return nw;
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

        void IWebViewLifeCycleManager.Display(WpfCefBrowser webview)
        {
            webview.Visibility = Visibility.Visible;
        }

        void IWebViewLifeCycleManager.Dispose(WpfCefBrowser ioldwebview)
        {
            //var wb = (ioldwebview as WebControl);
            ioldwebview.Visibility = Visibility.Hidden;

            //if (!ioldwebview.IsCrashed)
            //{
            //    ioldwebview.Source = new Uri("about:blank");
            //}

            //WebCore.ShuttingDown -= WebCore_ShuttingDown;
            this.MainGrid.Children.Remove(ioldwebview);

            ioldwebview.Dispose();

        }
    }
}