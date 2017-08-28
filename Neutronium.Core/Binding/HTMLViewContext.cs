using System;
using System.Threading.Tasks;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Binding
{
    public class HtmlViewContext : IDisposable
    {
        public IWebView WebView => _IWebBrowserWindow.MainFrame;
        public IDispatcher UiDispatcher { get; }
        public IJavascriptSessionInjector JavascriptSessionInjector { get; private set; }
        public IJavascriptViewModelUpdater ViewModelUpdater { get; private set; }
        public bool JavascriptFrameworkIsMappingObject => _JavascriptFrameworkManager.IsMappingObject;

        private IJavascriptObject _Listener;
        private IJavascriptViewModelManager _VmManager;
        private readonly IWebSessionLogger _Logger;
        private readonly IJavascriptChangesObserver _JavascriptChangesObserver;
        private readonly IJavascriptFrameworkManager _JavascriptFrameworkManager;
        private readonly IWebBrowserWindow _IWebBrowserWindow;

        public HtmlViewContext(IWebBrowserWindow webBrowserWindow, IDispatcher uiDispatcher, IJavascriptFrameworkManager javascriptFrameworkManager,
                                IJavascriptChangesObserver javascriptChangesObserver, IWebSessionLogger logger)
        {
            _IWebBrowserWindow = webBrowserWindow;
            _Logger = logger;
            UiDispatcher = uiDispatcher;
            _JavascriptChangesObserver = javascriptChangesObserver;
            _JavascriptFrameworkManager = javascriptFrameworkManager;
        }

        public void InitOnJsContext(bool debugMode)
        {
            var builder = new BinderBuilder(WebView, _JavascriptChangesObserver);
            _Listener = builder.BuildListener();
            _VmManager = _JavascriptFrameworkManager.CreateManager(WebView, _Listener, _Logger, debugMode);
            ViewModelUpdater = _VmManager.ViewModelUpdater;
            JavascriptSessionInjector = _VmManager.Injector;
        }

        public Task RunOnUIContextAsync(Action act) 
        {
            return UiDispatcher.RunAsync(act);
        }

        public Task RunOnJavascriptContextAsync(Action act)
        {
            return WebView.RunAsync(act);
        }

        public Task<T> EvaluateOnUIContextAsync<T>(Func<T> act)
        {
            return UiDispatcher.EvaluateAsync(act);
        }

        public bool IsTypeBasic(Type type) 
        {
            return WebView.Factory.IsTypeBasic(type);
        }

        public void Dispose() 
        {
            _VmManager.Dispose();
            _Listener.Dispose();
            _Logger.Debug("HTMLViewContext Disposed");
        }
    }
}
