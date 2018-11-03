using System;
using System.Threading.Tasks;
using Neutronium.Core.Binding;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using Neutronium.Core.Infra.VM;
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
        private IHtmlBinding _HTMLBinding;
        private IWebSessionLogger _webSessionLogger;
        private bool _Disposed = false;
        private bool _Navigating = false;
        private bool _UseINavigable = false;
        private HtmlLogicWindow _Window;

        public Uri Url { get; private set; }
        public IWebBrowserWindowProvider WebControl => _CurrentWebControl;
        public IWebBrowserWindow HTMLWindow => _CurrentWebControl?.HtmlWindow;
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

        public IHtmlBinding Binding
        {
            get { return _HTMLBinding; }
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

        private void Switch(Task<IHtmlBinding> binding, HtmlLogicWindow window, TaskCompletionSource<IHtmlBinding> tcs)
        {
            var oldvm = GetMainViewModel(Binding);
            var fireFirstLoad = false;
            Binding = binding.Result;

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

            var inav = _UseINavigable ? rootVm as INavigable : null;
            if (inav != null)
                inav.Navigation = this;
            _Window.State = WindowLogicalState.Opened;

            _Window.OpenAsync().ContinueWith(t => EndAnimation(rootVm));

            _Navigating = false;

            FireNavigate(rootVm, oldvm);

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

            _Navigating = true;

            var oldvm = GetMainViewModel(Binding) as INavigable;

            if (_UseINavigable && (oldvm != null))
            {
                oldvm.Navigation = null;
            }

            if (_CurrentWebControl != null)
            {
                _CurrentWebControl.HtmlWindow.Crashed -= Crashed;
                if (_CurrentWebControl.HtmlWindow is IModernWebBrowserWindow modern)
                    modern.OnClientReload -= ModerWindow_OnClientReload;
            }

            var closetask = (_CurrentWebControl != null) ? _Window.CloseAsync() : TaskHelper.Ended();

            _NextWebControl = _WebViewLifeCycleManager.Create();
            _NextWebControl.HtmlWindow.ConsoleMessage += ConsoleMessage;

            var moderWindow = _NextWebControl.HtmlWindow as IModernWebBrowserWindow;

            var injectorFactory = GetInjectorFactory(uri);
            var engine = new HtmlViewEngine(_NextWebControl, injectorFactory, _webSessionLogger);

            var dataContext = new DataContextViewModel(viewModel);
            var initVm = HtmlBinding.GetBindingBuilder(engine, dataContext, mode);

            if (moderWindow != null)
            {
                var debugContext = _WebViewLifeCycleManager.DebugContext;
                EventHandler<BeforeJavascriptExcecutionArgs> before = null;
                before = (o, e) =>
                {
                    moderWindow.BeforeJavascriptExecuted -= before;
                    e.JavascriptExecutor(_JavascriptFrameworkManager.GetMainScript(debugContext));
                };
                moderWindow.BeforeJavascriptExecuted += before;
                moderWindow.OnClientReload += ModerWindow_OnClientReload;
            }
            var tcs = new TaskCompletionSource<IHtmlBinding>();

            EventHandler<LoadEndEventArgs> sourceupdate = null;
            sourceupdate = async (o, e) =>
            {
                _NextWebControl.HtmlWindow.LoadEnd -= sourceupdate;

                var builder = await initVm;
                await builder.CreateBinding(_WebViewLifeCycleManager.DebugContext).WaitWith(closetask, t => Switch(t, dataContext.Window, tcs)).ConfigureAwait(false);
            };

            Url = uri;
            _NextWebControl.HtmlWindow.LoadEnd += sourceupdate;
            _NextWebControl.HtmlWindow.NavigateTo(uri);

            return tcs.Task;
        }

        private void Crashed(object sender, BrowserCrashedArgs e)
        {
            _webSessionLogger.Error("WebView crashed trying recover");
            Reload(true);
        }

        private void ModerWindow_OnClientReload(object sender, ClientReloadArgs e)
        {
            _webSessionLogger.Error("Page changes detected reloading bindings.");
            Reload(false);
        }

        private void Reload(bool forceClean)
        {
            var dest = _CurrentWebControl.HtmlWindow.Url;
            var vm = GetMainViewModel(Binding);
            var mode = Binding.Mode;

            if (forceClean)
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

        public void ExcecuteJavascript(string code)
        {
            try
            {
                _CurrentWebControl?.HtmlWindow.MainFrame.ExecuteJavaScript(code);
            }
            catch (Exception e)
            {
                _webSessionLogger.Error($"Can not execute javascript: {code}, reason: {e}");
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
