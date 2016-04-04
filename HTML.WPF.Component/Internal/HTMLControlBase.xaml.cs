using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using MVVM.HTML.Core;
using MVVM.HTML.Core.Infra.VM;
using MVVM.HTML.Core.Navigation;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.Window;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace HTML_WPF.Component
{
    public partial class HTMLControlBase : IWebViewLifeCycleManager, IDisposable
    {
        private IWPFWebWindowFactory _WPFWebWindowFactory;
        private IWebSessionWatcher _WebSessionWatcher;
        private IUrlSolver _UrlSolver;
        private DoubleBrowserNavigator _WPFDoubleBrowserNavigator;
        private string _JavascriptDebugScript = null;
        private readonly IJavascriptUIFrameworkManager _Injector;

        public ICommand DebugWindow { get; private set; }
        public ICommand DebugBrowser { get; private set; }

        public bool IsDebug
        {
            get { return (bool)GetValue(IsDebugProperty); }
            set { SetValue(IsDebugProperty, value); }
        }

        public static readonly DependencyProperty IsDebugProperty =
            DependencyProperty.Register("IsDebug", typeof(bool), typeof(HTMLControlBase), new PropertyMetadata(false));

        public bool IsHTMLLoaded
        {
            get { return (bool)GetValue(IsHTMLLoadedProperty); }
            private set { SetValue(IsHTMLLoadedProperty, value); }
        }

        public static readonly DependencyProperty IsHTMLLoadedProperty =
            DependencyProperty.Register("IsHTMLLoaded", typeof(bool), typeof(HTMLControlBase), new PropertyMetadata(false));

        public string HTMLEngine
        {
            get { return (string)GetValue(HTMLEngineProperty); }
            set { SetValue(HTMLEngineProperty, value); }
        }

        public static readonly DependencyProperty HTMLEngineProperty =
            DependencyProperty.Register("HTMLEngine", typeof(string), typeof(HTMLControlBase), new PropertyMetadata(string.Empty));


        public Uri Source
        {
            get { return _WPFDoubleBrowserNavigator.Url; }
        }

        public bool UseINavigable
        {
            get { return _WPFDoubleBrowserNavigator.UseINavigable; }
            set { _WPFDoubleBrowserNavigator.UseINavigable = value; }
        }

        protected HTMLControlBase(IUrlSolver urlSolver)
        {
            _UrlSolver = urlSolver;

            DebugWindow = new BasicRelayCommand(ShowDebugWindow);
            DebugBrowser = new BasicRelayCommand(OpenDebugBrowser);

            InitializeComponent();

            var engine = HTMLEngineFactory.Engine;
            _WPFWebWindowFactory = engine.ResolveJavaScriptEngine(HTMLEngine);
            _Injector = engine.ResolveJavaScriptFramework(HTMLEngine);
            WebSessionWatcher = engine.WebSessionWatcher;

            _WPFDoubleBrowserNavigator = new DoubleBrowserNavigator(this, _UrlSolver, _Injector);
            _WPFDoubleBrowserNavigator.OnFirstLoad += FirstLoad;
        }

        public IWebSessionWatcher WebSessionWatcher
        {
            get { return _WebSessionWatcher; }
            set { _WebSessionWatcher = value; _WPFDoubleBrowserNavigator.WebSessionWatcher = value; }
        }

        private void FirstLoad(object sender, EventArgs e)
        {
            IsHTMLLoaded = true;
            _WPFDoubleBrowserNavigator.OnFirstLoad -= FirstLoad;
        }
       
        private void RunKoView()
        {
            if (_JavascriptDebugScript == null)
            {
                _JavascriptDebugScript = _Injector.GetDebugScript();
            }
            _WPFDoubleBrowserNavigator.ExcecuteJavascript(_JavascriptDebugScript);
        }

        public void ShowDebugWindow()
        {
            RunKoView();
            _WPFDoubleBrowserNavigator.ExcecuteJavascript("ko.dodebug();");
        }

        public void OpenDebugBrowser() 
        {
            var currentWebControl = _WPFDoubleBrowserNavigator.WebControl;
            if (currentWebControl == null) 
                return;

            var result = currentWebControl.OnDebugToolsRequest();
            if (!result)
                MessageBox.Show("Debug tools not available!");
        }

        public void CloseDebugBrowser() 
        {
            var currentWebControl = _WPFDoubleBrowserNavigator.WebControl;
            if (currentWebControl == null)
                return;

            currentWebControl.CloseDebugTools();
        }

        protected async Task<IHTMLBinding> NavigateAsyncBase(object iViewModel, string Id = null, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay)
        {
            return await _WPFDoubleBrowserNavigator.NavigateAsync(iViewModel, Id, iMode);
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
            if (_WPFWebWindowFactory == null)
            {
                _WPFWebWindowFactory = HTMLEngineFactory.Engine.ResolveJavaScriptEngine(HTMLEngine);

                if (_WPFWebWindowFactory==null)
                    throw ExceptionHelper.Get(string.Format("Not able to find WebEngine {0}", HTMLEngine));
            }

            var webwindow = _WPFWebWindowFactory.Create();           
            var ui = webwindow.UIElement;
            Panel.SetZIndex(ui, 0);
            Grid.SetColumnSpan(ui, 2);

            if (!webwindow.IsUIElementAlwaysTopMost)
                Grid.SetRowSpan(ui, 2);
            
            this.MainGrid.Children.Add(ui);
            return new WPFHTMLWindowProvider(webwindow, this );
        }

        IDispatcher IWebViewLifeCycleManager.GetDisplayDispatcher()
        {
            return new WPFUIDispatcher(this.Dispatcher);
        }

        public void Inject(Key keyToInject)
        {
            var wpfacess =  (_WPFDoubleBrowserNavigator.WebControl as WPFHTMLWindowProvider);
            if (wpfacess == null)
                return;

            var wpfweb = wpfacess.IWPFWebWindow;            
            if (wpfweb!=null)
                wpfweb.Inject(keyToInject);
        }
    }
}
