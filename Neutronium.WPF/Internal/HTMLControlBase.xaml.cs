using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Neutronium.Core;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra.VM;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.Navigation;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WPF.Internal.DebugTools;
using Neutronium.WPF.Utils;
using Microsoft.Win32;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.WPF.Internal
{
    public partial class HTMLControlBase : IWebViewLifeCycleManager, IDisposable
    {
        private UserControl _DebugControl;
        private DebugInformation _DebugInformation;
        private IWPFWebWindowFactory _WPFWebWindowFactory;
        private IWebSessionLogger _webSessionLogger;
        private readonly IUrlSolver _UrlSolver;
        private IJavascriptFrameworkManager _Injector;
        private string _SaveDirectory;
        private bool _LoadingAbout = false;
        private HTMLSimpleWindow _VmDebugWindow;
        private DoubleBrowserNavigator _WPFDoubleBrowserNavigator;
        private Window Window => Window.GetWindow(this);
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

        public static readonly DependencyProperty IsDebugProperty = DependencyProperty.Register(nameof(IsDebug), typeof(bool), typeof(HTMLControlBase), new PropertyMetadata(false, DebugChanged));

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

        private bool _UseINavigable = true;
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

            _DebugInformation = new DebugInformation
            {
                DebugWindow = new BasicRelayCommand(ShowDebugWindow),
                DebugBrowser = new BasicRelayCommand(OpenDebugBrowser),
                ShowInfo = new BasicRelayCommand(DoShowInfo),
                SaveVm = new BasicRelayCommand(DoSaveVm),
                IsDebuggingVm = false,
                NeutroniumWPFVersion = VersionHelper.GetVersion(this).GetDisplayName(),
                ComponentName = this.GetType().Name
            };

            InitializeComponent();

            this.Initialized += HTMLControlBase_Initialized;
        }

        private void DoSaveVm()
        {
            var binding = _WPFDoubleBrowserNavigator?.Binding?.JSBrideRootObject;
            if (binding == null)
                return;

            var savefile = new SaveFileDialog 
            {
                FileName = "vm.cjson",
                InitialDirectory = ComputeProposedDirectory()
            };

            if (savefile.ShowDialog() != true)
                return;

            var fileName = savefile.FileName;
            _SaveDirectory = Path.GetDirectoryName(fileName);
            var descriptionBuilder = new DescriptionBuilder("null");
            binding.BuilString(descriptionBuilder);
            var content = descriptionBuilder.BuildString();
            File.WriteAllLines(fileName, new[] { content });
        }

        private string ComputeProposedDirectory()
        {
            var path = _WPFDoubleBrowserNavigator?.HTMLWindow?.Url?.AbsolutePath;
            if (path == null)
                return _SaveDirectory;

            var directory = Path.GetDirectoryName(path); ;
            var transformed = directory.Replace(@"\bin\Debug", string.Empty);
            transformed = transformed.Replace(@"\bin\Release", string.Empty);

            if (transformed.Length == directory.Length)
                return _SaveDirectory;

            var dataFolder = Path.Combine(Path.GetDirectoryName(transformed), "data");
            return Directory.Exists(dataFolder) ? dataFolder : _SaveDirectory;
        }

        private void DebugChanged(bool isDebug)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            if (_WPFWebWindowFactory == null)
                return;

            if (isDebug)
            {
                SetUpDebugTool();
            }
            else
            {
                MainGrid.Children.Remove(_DebugControl);
                _DebugControl = null;
            }
        }

        private void SetUpDebugTool() 
        {
            if (_DebugControl!=null)
                return;

            var windoInfo = HTMLEngineFactory.Engine.ResolveToolbar();
            if (windoInfo != null)
            {
                _DebugControl = new DebugControlNeutronium(windoInfo.AbsolutePath, windoInfo.Framework.Name);
            }
            else
            {
                _DebugControl = new DebugControl();
            }
            _DebugControl.DataContext = _DebugInformation;
            Grid.SetRow(_DebugControl, 1);
            MainGrid.Children.Add(_DebugControl);
        }

        private void HTMLControlBase_Initialized(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            if (_WPFWebWindowFactory != null)
                return;

            var engine = HTMLEngineFactory.Engine;
            _WPFWebWindowFactory = engine.ResolveJavaScriptEngine(HTMLEngine);

            if (_WPFWebWindowFactory == null)
                throw ExceptionHelper.Get($"Not able to find WebEngine {HTMLEngine}");

            _Injector = engine.ResolveJavaScriptFramework(JavascriptUIEngine);

            if (_Injector == null)
                throw ExceptionHelper.Get($"Not able to find JavascriptUIEngine {JavascriptUIEngine}. Please register the correspoding Javascript UIEngine.");

            _WPFDoubleBrowserNavigator = GetDoubleBrowserNavigator();

            WebSessionLogger = WebSessionLogger?? engine.WebSessionLogger;

            if (IsDebug)
                SetUpDebugTool();
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
            var windoInfo = HTMLEngineFactory.Engine.ResolveAboutScreen();
            if (windoInfo != null) 
            {
                if (_LoadingAbout)
                    return;

                _LoadingAbout = true;
                var aboutWindow = new NeutroniumWindow(windoInfo.AbsolutePath, windoInfo.Framework.Name)
                {
                    Title = "About",
                    Owner = Window,
                    DataContext = new About(_WPFWebWindowFactory, _Injector),
                    Width = windoInfo.Width,
                    Height = windoInfo.Height
                };
                EventHandler handler = null;
                handler = (o, e) =>
                {
                    aboutWindow.OnFirstLoad -= handler;
                    aboutWindow.ShowDialog();
                    _LoadingAbout = false;
                };
                aboutWindow.OnFirstLoad += handler;
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine($"Neutronium.Core { VersionHelper.GetVersion(typeof(IHTMLBinding)).GetDisplayName()}")
                   .AppendLine($"WebBrowser: {_WPFWebWindowFactory.EngineName}")
                   .AppendLine($"Browser binding: {_WPFWebWindowFactory.Name}")
                   .AppendLine($"Javascript Framework: {_Injector.FrameworkName}")
                   .AppendLine($"MVVM Binding: {_Injector.Name}");
            MessageBox.Show(Window, builder.ToString(), "Neutronium configuration");
        }

        private HTMLSimpleWindow GetWHMLWindow(string path, string title, Func<IWebView, IDisposable> onWebViewCreated = null) 
        {
            return new HTMLSimpleWindow(_WPFWebWindowFactory.Create(), path, onWebViewCreated) 
            {
                Owner = Window,
                Title = title
            };
        }

        public IWebSessionLogger WebSessionLogger
        {
            get { return _webSessionLogger; }
            set
            {
                _webSessionLogger = value;
                if (_WPFDoubleBrowserNavigator != null)
                    _WPFDoubleBrowserNavigator.WebSessionLogger = value;
            }
        }

        public void ShowDebugWindow()
        {
            if (_VmDebugWindow != null)
            {
                _VmDebugWindow.Activate();
                return;
            }
            _Injector.DebugVm(script => WPFDoubleBrowserNavigator.ExcecuteJavascript(script), 
                                (path, onCreate) => ShowHTMLWindow(path, debug => onCreate(WPFDoubleBrowserNavigator.HTMLWindow.MainFrame, debug)));

            if (_VmDebugWindow == null)
                _DebugInformation.IsDebuggingVm = !_DebugInformation.IsDebuggingVm;
            else
                _DebugInformation.IsDebuggingVm = true;
        }

        private void ShowHTMLWindow(string path, Func<IWebView, IDisposable> injectedCode) 
        {
            _VmDebugWindow = GetWHMLWindow(path, "Neutronium ViewModel Debugger", injectedCode);
            _VmDebugWindow.Closed += _VmDebugWindow_Closed;
            _VmDebugWindow.Show();
        }

        private void _VmDebugWindow_Closed(object sender, EventArgs e)
        {
            _VmDebugWindow.Closed -= _VmDebugWindow_Closed;
            _VmDebugWindow = null;
            _DebugInformation.IsDebuggingVm = false;
        }

        public void OpenDebugBrowser()
        {
            var currentWebControl = WPFDoubleBrowserNavigator.WebControl;
            if (currentWebControl == null)
                return;

            WPFDoubleBrowserNavigator.WebControl.DebugToolOpened += WebControl_DebugToolOpened;

            var result = currentWebControl.OnDebugToolsRequest();
            if (!result)
                MessageBox.Show("Debug tools not available!");
        }

        private void WebControl_DebugToolOpened(object sender, DebugEventArgs eventArgs)
        {
            _DebugInformation.IsInspecting = eventArgs.Opening;
            if (!eventArgs.Opening)
            {
                WPFDoubleBrowserNavigator.WebControl.DebugToolOpened -= WebControl_DebugToolOpened;
            }
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
            var res = new WPFHTMLWindowProvider(webwindow, this);
            res.OnDisposed += OnWindowDisposed;
            return res;
        }

        private void OnWindowDisposed(object sender, EventArgs e) 
        {
            var res = sender as WPFHTMLWindowProvider;
            res.OnDisposed -= OnWindowDisposed;
            if (_VmDebugWindow != null) 
            {
                _VmDebugWindow.Close();
                _VmDebugWindow = null;
            }
        }

        IDispatcher IWebViewLifeCycleManager.GetDisplayDispatcher()
        {
            return new WPFUIDispatcher(this.Dispatcher);
        }

        public void Inject(Key keyToInject)
        {
            var wpfacess = (WPFDoubleBrowserNavigator.WebControl as WPFHTMLWindowProvider);
            if (wpfacess == null)
                return;

            var wpfweb = wpfacess.WPFWebWindow;
            wpfweb?.Inject(keyToInject);
        }
    }
}
