using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Neutronium.Core;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra.VM;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.Navigation;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.WPF.Internal
{
    public partial class HTMLControlBase : IWebViewLifeCycleManager, IDisposable
    {
        private IWPFWebWindowFactory _WPFWebWindowFactory;
        private IWebSessionLogger _webSessionLogger;
        private string _JavascriptDebugScript = null;
        private readonly IUrlSolver _UrlSolver;
        private DoubleBrowserNavigator _WPFDoubleBrowserNavigator;
        private DoubleBrowserNavigator WPFDoubleBrowserNavigator
        {
            get
            {
                Init();
                return _WPFDoubleBrowserNavigator;
            }
        }


        private IJavascriptFrameworkManager _Injector;

        public BasicRelayCommand DebugWindow { get; }
        public BasicRelayCommand DebugBrowser { get; }
        public BasicRelayCommand ShowInfo { get; }

        public bool DebugContext => IsDebug;
        public Uri Source => _WPFDoubleBrowserNavigator?.Url;

        public bool IsDebug
        {
            get { return (bool)GetValue(IsDebugProperty); }
            set { SetValue(IsDebugProperty, value); }
        }

        public static readonly DependencyProperty IsDebugProperty = DependencyProperty.Register(nameof(IsDebug), typeof (bool), typeof (HTMLControlBase), new PropertyMetadata(false));

        public bool VmDebug
        {
            get { return (bool) GetValue(VmDebugProperty); }
            private set { SetValue(VmDebugProperty, value); }
        }

        public static readonly DependencyProperty VmDebugProperty = DependencyProperty.Register(nameof(VmDebug), typeof(bool), typeof(HTMLControlBase), new PropertyMetadata(false));

        public bool IsHTMLLoaded
        {
            get { return (bool)GetValue(IsHTMLLoadedProperty); }
            private set { SetValue(IsHTMLLoadedProperty, value); }
        }

        public static readonly DependencyProperty IsHTMLLoadedProperty =
            DependencyProperty.Register(nameof(IsHTMLLoaded), typeof(bool), typeof(HTMLControlBase), new PropertyMetadata(false));

        public string HTMLEngine
        {
            get { return (string)GetValue(HTMLEngineProperty); }
            set { SetValue(HTMLEngineProperty, value); }
        }

        public static readonly DependencyProperty HTMLEngineProperty =
            DependencyProperty.Register(nameof(HTMLEngine), typeof(string), typeof(HTMLControlBase), new PropertyMetadata(string.Empty));

        public string JavascriptUIEngine
        {
            get { return (string)GetValue(JavascriptUIEngineProperty); }
            set { SetValue(JavascriptUIEngineProperty, value); }
        }

        public static readonly DependencyProperty JavascriptUIEngineProperty =
            DependencyProperty.Register(nameof(JavascriptUIEngine), typeof(string), typeof(HTMLControlBase), new PropertyMetadata(string.Empty));

        private bool _UseINavigable=true;
        public bool UseINavigable
        {
            get { return _UseINavigable; }
            set 
            {
                _UseINavigable = value;
                if (_WPFDoubleBrowserNavigator != null) 
                {
                    _WPFDoubleBrowserNavigator.UseINavigable = _UseINavigable;
                }         
            }
        }

        public event EventHandler<NavigationEvent> OnNavigate;
        public event EventHandler OnFirstLoad;
        public event EventHandler<DisplayEvent> OnDisplay;

        protected HTMLControlBase(IUrlSolver urlSolver) 
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            _UrlSolver = urlSolver;

            DebugWindow = new BasicRelayCommand(ShowDebugWindow);
            DebugBrowser = new BasicRelayCommand(OpenDebugBrowser);
            ShowInfo = new BasicRelayCommand(DoShowInfo);

            InitializeComponent();

            this.Loaded += HTMLControlBase_Loaded;    
        }

        private void HTMLControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            if (_WPFWebWindowFactory != null)
                return;

            if (IsLoaded==false)
                throw ExceptionHelper.Get($"Not able to access Neutronium methods before the component is loaded");

            var engine = HTMLEngineFactory.Engine;
            _WPFWebWindowFactory = engine.ResolveJavaScriptEngine(HTMLEngine);

            if (_WPFWebWindowFactory == null)
                throw ExceptionHelper.Get($"Not able to find WebEngine {HTMLEngine}");

            _Injector = engine.ResolveJavaScriptFramework(JavascriptUIEngine);

            if (_Injector == null)
                throw ExceptionHelper.Get($"Not able to find JavascriptUIEngine {JavascriptUIEngine}. Please register the correspoding Javascript UIEngine.");

            var debugableVm = _Injector.HasDebugScript();
            DebugWindow.Executable = debugableVm;
            VmDebug = debugableVm;

            _WPFDoubleBrowserNavigator = GetDoubleBrowserNavigator();

            WebSessionLogger = engine.WebSessionLogger;
        }

        private DoubleBrowserNavigator GetDoubleBrowserNavigator()
        {
            var wpfDoubleBrowserNavigator = new DoubleBrowserNavigator(this, _UrlSolver, _Injector);
            wpfDoubleBrowserNavigator.OnFirstLoad += FirstLoad;
            wpfDoubleBrowserNavigator.OnNavigate += OnNavigateFired;
            wpfDoubleBrowserNavigator.OnDisplay += OnDisplayFired;
            wpfDoubleBrowserNavigator.UseINavigable = _UseINavigable;
            return wpfDoubleBrowserNavigator;
        }

        private void FirstLoad(object sender, EventArgs e)
        {
            IsHTMLLoaded = true;
            OnFirstLoad?.Invoke(this, e);
        }

        private void OnNavigateFired(object sender, NavigationEvent e)
        {
            OnNavigate?.Invoke(this, e);
        }

        private void OnDisplayFired(object sender, DisplayEvent e)
        {
            OnDisplay?.Invoke(this, e);
        }

        private void DoShowInfo()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Neutronium version: {typeof(IHTMLBinding).Assembly.GetName().Version}")
                   .AppendLine($"WebBrowser: {_WPFWebWindowFactory.EngineName}")
                   .AppendLine($"Browser binding: {_WPFWebWindowFactory.Name}")
                   .AppendLine($"Javascript Framework: {_Injector.FrameworkName}")
                   .AppendLine($"MVVM Binding: {_Injector.Name}");
            MessageBox.Show(GetParentWindow(), builder.ToString(), "Neutronium configuration");
        }

        private Window GetParentWindow() 
        {
            var parent = VisualTreeHelper.GetParent(this);
            while (!(parent is Window)) 
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as Window;
        }

        public IWebSessionLogger WebSessionLogger
        {
            get { return _webSessionLogger; }
            set { _webSessionLogger = value; _WPFDoubleBrowserNavigator.WebSessionLogger = value; }
        }
       
        private void RunDebugscript()
        {
            if (_JavascriptDebugScript == null)
            {
                _JavascriptDebugScript = _Injector.GetDebugScript();
            }
            WPFDoubleBrowserNavigator.ExcecuteJavascript(_JavascriptDebugScript);
        }

        public void ShowDebugWindow()
        {
            RunDebugscript();
            WPFDoubleBrowserNavigator.ExcecuteJavascript(_Injector.GetDebugToogleScript());
        }

        public void OpenDebugBrowser() 
        {
            var currentWebControl = WPFDoubleBrowserNavigator.WebControl;
            if (currentWebControl == null) 
                return;

            var result = currentWebControl.OnDebugToolsRequest();
            if (!result)
                MessageBox.Show("Debug tools not available!");
        }

        public void CloseDebugBrowser() 
        {
            var currentWebControl = WPFDoubleBrowserNavigator.WebControl;
            currentWebControl?.CloseDebugTools();
        }

        protected async Task<IHTMLBinding> NavigateAsyncBase(object iViewModel, string Id = null, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay)
        {
            return await WPFDoubleBrowserNavigator.NavigateAsync(iViewModel, Id, iMode);
        }

        public void Dispose()
        {
            _WPFDoubleBrowserNavigator.Dispose();
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

        IWebBrowserWindowProvider IWebViewLifeCycleManager.Create()
        {
            var webwindow = _WPFWebWindowFactory.Create();           
            var ui = webwindow.UIElement;
            Panel.SetZIndex(ui, 0);
            
            this.MainGrid.Children.Add(ui);
            return new WPFHTMLWindowProvider(webwindow, this );
        }

        IDispatcher IWebViewLifeCycleManager.GetDisplayDispatcher()
        {
            return new WPFUIDispatcher(this.Dispatcher);
        }

        public void Inject(Key keyToInject)
        {
            var wpfacess =  (WPFDoubleBrowserNavigator.WebControl as WPFHTMLWindowProvider);
            if (wpfacess == null)
                return;

            var wpfweb = wpfacess.IWPFWebWindow;
            wpfweb?.Inject(keyToInject);
        }
    }
}
