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
using Neutronium.WPF.Internal.DebugViewModel;
using Neutronium.WPF.Utils;

namespace Neutronium.WPF.Internal
{
    public partial class HTMLControlBase : IWebViewLifeCycleManager, IDisposable
    {
        private UserControl _DebugControl;
        private DebugInformation _DebugInformation;
        private IWPFWebWindowFactory _WPFWebWindowFactory;
        private IWebSessionLogger _webSessionLogger;
        private string _JavascriptDebugScript = null;
        private readonly IUrlSolver _UrlSolver;
        private IJavascriptFrameworkManager _Injector;
        private DoubleBrowserNavigator _WPFDoubleBrowserNavigator;
        private DoubleBrowserNavigator WPFDoubleBrowserNavigator
        {
            get
            {
                Init();
                return _WPFDoubleBrowserNavigator;
            }
        }

        public bool DebugContext => IsDebug;
        public Uri Source => _WPFDoubleBrowserNavigator?.Url;
        public IWPFWebWindow WPFWebWindow => (WPFDoubleBrowserNavigator.WebControl as WPFHTMLWindowProvider)?.WPFWebWindow;

        public bool IsDebug
        {
            get { return (bool)GetValue(IsDebugProperty); }
            set { SetValue(IsDebugProperty, value); }
        }

        public static readonly DependencyProperty IsDebugProperty = DependencyProperty.Register(nameof(IsDebug), typeof (bool), typeof (HTMLControlBase), new PropertyMetadata(false, DebugChanged));

        private static void DebugChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as HTMLControlBase;
            control.DebugChanged((bool)e.NewValue);
        }

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

            _DebugInformation = new DebugInformation {
                DebugWindow = new BasicRelayCommand(ShowDebugWindow),
                DebugBrowser = new BasicRelayCommand(OpenDebugBrowser),
                ShowInfo = new BasicRelayCommand(DoShowInfo),
                IsDebuggingVm = false,
                NeutroniumWPFVersion =  VersionHelper.GetVersion(this).GetDisplayName(),
                ComponentName = this.GetType().Name
            };

            InitializeComponent();

            this.Loaded += HTMLControlBase_Loaded;    
        }

        private void DebugChanged(bool isDebug)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            if (isDebug)
            {
                var neutroniumSupport = HTMLEngineFactory.Engine.HasJavaScriptEngine("VueInjectorV2");
                if (neutroniumSupport)
                {
                    _DebugControl = new DebugControlNeutronium();
                }
                else
                {
                    _DebugControl = new DebugControl();
                }
                _DebugControl.DataContext = _DebugInformation;
                Grid.SetRow(_DebugControl, 1);
                MainGrid.Children.Add(_DebugControl);
            }
            else
            {
                MainGrid.Children.Remove(_DebugControl);
                _DebugControl = null;
            }
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
            _DebugInformation.SetVmDebug(debugableVm);

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
            builder.AppendLine($"Neutronium.Core { VersionHelper.GetVersion(typeof(IHTMLBinding)).GetDisplayName()}")
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
            set {
                _webSessionLogger = value;
                if (_WPFDoubleBrowserNavigator!=null)
                    _WPFDoubleBrowserNavigator.WebSessionLogger = value;
            }
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
            _DebugInformation.IsDebuggingVm = !_DebugInformation.IsDebuggingVm;
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
            (_DebugControl as IDisposable)?.Dispose();
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

            var wpfweb = wpfacess.WPFWebWindow;
            wpfweb?.Inject(keyToInject);
        }
    }
}
