using Neutronium.Core.Binding;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.VM;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.Log;
using Neutronium.Core.Navigation.Window;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;
using System;
using System.Threading.Tasks;

namespace Neutronium.Core.Navigation
{
    public class DoubleBrowserNavigator : INavigationSolver, IWebViewComponent
    {
        private readonly IWebViewLifeCycleManager _WebViewLifeCycleManager;
        private readonly IJavascriptFrameworkManager _JavascriptFrameworkManager;
        private readonly IUrlSolver _UrlSolver;
        private Uri CurrentUrl => _CurrentWebControl?.HtmlWindow?.Url ?? Url;
        private IWebBrowserWindowProvider _CurrentWebControl;
        private IWebBrowserWindowProvider _NextWebControl;
        private IHtmlBinding _HTMLBinding;
        private IWebSessionLogger _WebSessionLogger;
        private bool _Disposed = false;
        private bool _Navigating = false;
        private HtmlLogicWindow _Window;

        public Uri Url { get; private set; }
        public IWebBrowserWindowProvider WebControl => _CurrentWebControl;
        public IWebBrowserWindow HTMLWindow => _CurrentWebControl?.HtmlWindow;
        public bool UseINavigable { get; set; } = false;

        public IWebSessionLogger WebSessionLogger
        {
            set => _WebSessionLogger = value;
        }

        public DoubleBrowserNavigator(IWebViewLifeCycleManager lifeCycleManager, IUrlSolver urlSolver, IJavascriptFrameworkManager javascriptFrameworkManager)
        {
            _WebSessionLogger = new BasicLogger();
            _JavascriptFrameworkManager = javascriptFrameworkManager;
            _WebViewLifeCycleManager = lifeCycleManager;
            _UrlSolver = urlSolver;
        }

        private void ConsoleMessage(object sender, ConsoleMessageArgs e)
        {
            try
            {
                _WebSessionLogger.LogBrowser(e, Url);
            }
            catch
            {
                // ignored
            }
        }

        public IHtmlBinding Binding
        {
            get => _HTMLBinding;
            private set
            {
                _HTMLBinding?.Dispose();

                _HTMLBinding = value;

                if (_Disposed && (_HTMLBinding != null))
                    Binding = null;
            }
        }

        private void FireNavigate(object newVm, object oldVm = null)
        {
            OnNavigate?.Invoke(this, new NavigationEvent(newVm, oldVm));
        }

        private void FireLoaded(object loadedVm)
        {
            OnDisplay?.Invoke(this, new DisplayEvent(loadedVm));
        }

        private void Switch(IHtmlBinding binding, HtmlLogicWindow window, TaskCompletionSource<IHtmlBinding> tcs)
        {
            var oldViewModel = GetMainViewModel(Binding);
            var fireFirstLoad = false;
            Binding = binding;

            if (_CurrentWebControl != null)
            {
                _CurrentWebControl.HtmlWindow.ConsoleMessage -= ConsoleMessage;
                _CurrentWebControl.Dispose();
            }
            else
            {
                fireFirstLoad = true;
            }

            _CurrentWebControl = _NextWebControl;
            _NextWebControl = null;
            _CurrentWebControl.HtmlWindow.Crashed += Crashed;

            _CurrentWebControl.Show();

            _Window = window;

            var rootVm = GetMainViewModel(Binding);

            var navigable = UseINavigable ? rootVm as INavigable : null;
            if (navigable != null)
                navigable.Navigation = this;
            _Window.State = WindowLogicalState.Opened;

            _Window.OpenAsync().ContinueWith(t => EndAnimation(rootVm));

            _Navigating = false;

            FireNavigate(rootVm, oldViewModel);

            tcs?.SetResult(Binding);

            if (!fireFirstLoad)
                return;

            OnFirstLoad?.Invoke(this, new FirstLoadEvent(_CurrentWebControl.HtmlWindow));
        }

        private static object GetMainViewModel(IHtmlBinding binding)
        {
            return ((DataContextViewModel)(binding?.Root))?.ViewModel;
        }

        private void EndAnimation(object navigable)
        {
            _WebViewLifeCycleManager.GetDisplayDispatcher().Dispatch(() => FireLoaded(navigable));
        }

        private Task<IHtmlBinding> Navigate(Uri uri, object viewModel, JavascriptBindingMode mode = JavascriptBindingMode.TwoWay)
        {
            if (uri == null)
                throw ExceptionHelper.GetArgument($"ViewModel not registered: {viewModel.GetType()}");

            if (_Navigating)
                return null;

            _Navigating = true;

            var oldViewModel = GetMainViewModel(Binding) as INavigable;

            if (UseINavigable && (oldViewModel != null))
            {
                oldViewModel.Navigation = null;
            }

            if (_CurrentWebControl != null)
            {
                _CurrentWebControl.HtmlWindow.Crashed -= Crashed;
                if (_CurrentWebControl.HtmlWindow is IModernWebBrowserWindow modern)
                    modern.OnClientReload -= ModernWindow_OnClientReload;
            }

            var closeTask = (_CurrentWebControl != null) ? _Window.CloseAsync() : TaskHelper.Ended();

            _NextWebControl = _WebViewLifeCycleManager.Create();
            _NextWebControl.HtmlWindow.ConsoleMessage += ConsoleMessage;

            var modernWindow = _NextWebControl.HtmlWindow as IModernWebBrowserWindow;

            var injectorFactory = GetInjectorFactory(uri);
            var engine = new HtmlViewEngine(_NextWebControl, injectorFactory, _WebSessionLogger);

            var dataContext = new DataContextViewModel(viewModel);
            var builder = HtmlBinding.GetBindingBuilder(engine, dataContext, mode);

            if (modernWindow != null)
            {
                var debugContext = _WebViewLifeCycleManager.DebugContext;

                void Before(object o, BeforeJavascriptExcecutionArgs e)
                {
                    modernWindow.BeforeJavascriptExecuted -= Before;
                    e.JavascriptExecutor(_JavascriptFrameworkManager.GetMainScript(debugContext));
                }
                modernWindow.BeforeJavascriptExecuted += Before;
                modernWindow.OnClientReload += ModernWindow_OnClientReload;
            }
            var tcs = new TaskCompletionSource<IHtmlBinding>();
            var debug = _WebViewLifeCycleManager.DebugContext;

            async void SourceUpdate(object o, LoadEndEventArgs e)
            {
                _NextWebControl.HtmlWindow.LoadEnd -= SourceUpdate;
                var bind = await builder.CreateBinding(debug).WaitWith(closeTask);
                _NextWebControl.UiDispatcher.Dispatch(() =>
                    Switch(bind, dataContext.Window, tcs)
                );
            }

            Url = uri;
            _NextWebControl.HtmlWindow.LoadEnd += SourceUpdate;
            _NextWebControl.HtmlWindow.NavigateTo(uri);

            return tcs.Task;
        }

        public Task ReloadAsync()
        {
            _WebSessionLogger.Info("Reloading to same page.");
            return SafeReloadAsync();
        }

        public Task SwitchViewAsync(Uri target)
        {
            _WebSessionLogger.Info($"Switching to uri: {target}");
            var newUri = new UriBuilder(target)
            {
                Fragment = CurrentUrl.Fragment.Replace("#",String.Empty)
            };
            return SafeReloadAsync(newUri.Uri);
        }

        private Task SafeReloadAsync(Uri target= null)
        {
            var dispatcher = _CurrentWebControl?.UiDispatcher;
            return dispatcher?.RunAsync(() => Reload(true, target)) ?? Task.CompletedTask;
        }

        private void Crashed(object sender, BrowserCrashedArgs e)
        {
            _WebSessionLogger.Error("WebView crashed trying recover");
            Reload(false);
        }

        private void ModernWindow_OnClientReload(object sender, ClientReloadArgs e)
        {
            _WebSessionLogger.Info("Page changes detected reloading bindings.");
            Reload(true, new Uri(e.Url));
        }

        private void Reload(bool hotReloadContext, Uri url = null)
        {
            var dest = url ?? CurrentUrl;
            var vm = GetMainViewModel(Binding);
            var mode = Binding.Mode;

            if (!hotReloadContext)
            {
                CleanWebControl(ref _CurrentWebControl);
            }
            Binding = null;

            Navigate(dest, vm, mode);
        }

        private IJavascriptFrameworkManager GetInjectorFactory(Uri uri)
        {
            return _JavascriptFrameworkManager;
        }

        public void ExecuteJavascript(string code)
        {
            try
            {
                _CurrentWebControl?.HtmlWindow.MainFrame.ExecuteJavaScript(code);
            }
            catch (Exception e)
            {
                _WebSessionLogger.Error($"Can not execute javascript: {code}, reason: {e}");
            }
        }

        public async Task<IHtmlBinding> NavigateAsync(object viewModel, string id = "", JavascriptBindingMode mode = JavascriptBindingMode.TwoWay)
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

        private void CleanWebControl(ref IWebBrowserWindowProvider webControl)
        {
            if (webControl == null)
                return;

            webControl.HtmlWindow.Crashed -= Crashed;
            webControl.HtmlWindow.ConsoleMessage -= ConsoleMessage;
            webControl.Dispose();
            webControl = null;
        }

        public event EventHandler<NavigationEvent> OnNavigate;

        public event EventHandler<DisplayEvent> OnDisplay;

        public event EventHandler<FirstLoadEvent> OnFirstLoad;
    }
}
