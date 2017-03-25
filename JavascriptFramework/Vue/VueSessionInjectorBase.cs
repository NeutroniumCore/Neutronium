using System;
using System.Text;
using Neutronium.Core;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.JavascriptFramework.Vue.Communication;

namespace Neutronium.JavascriptFramework.Vue
{
    public class VueSessionInjectorBase : IJavascriptFrameworkManager
    {
        public string FrameworkName => "Vue.js";
        public string Name => _VueVersion.Name;
        public string FrameworkVersion => _VueVersion.FrameworkNameVersion;
        public DebugToolsUI DebugToolsUI => _VueVersion.DebugToolsUI;

        private readonly IVueVersion _VueVersion;
        private readonly IWebViewCommunication _WebViewCommunication;

        protected VueSessionInjectorBase(IVueVersion vueVersion)
        {
            _VueVersion = vueVersion;
            _WebViewCommunication = new WebViewCommunication();
        }

        public IJavascriptViewModelManager CreateManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger, bool debugMode)
        {
            return new VueVmManager(webView, listener, debugMode ? _WebViewCommunication : null, logger);
        }

        public void DebugVm(Action<string> runJavascript, Action<string, int, int, Func<IWebView, IWebView, IDisposable>> openNewWindow)
        {
            openNewWindow(@"DebugTools\Window\index.html", 800, 700, RegisterDebugWindowHook);
        }

        private IDisposable RegisterDebugWindowHook(IWebView current, IWebView debugWebView)
        {
            var disp = _WebViewCommunication.Connect(current, debugWebView);
            var disp2 = _WebViewCommunication.Subscribe(debugWebView, "main:inject", _ => InjectBackend(current));
            var disconnector = new DisposableAction(() => _WebViewCommunication.Disconnect(debugWebView));
            return new ComposedDisposable(disp, disp2, disconnector);
        }

        private void InjectBackend(IWebView current)
        {
            var loader = GetResourceReader("DebugTools.Window.dist");
            var data = loader.Load("backend.js");
            data = ";window.__neutronium_listener__.post('dev:injectDone');" + data;
            current.ExecuteJavaScript(data);
        }

        public string GetMainScript(bool debugMode)
        {
            var commonLoader = GetResourceReader();
            var versionLoader = _VueVersion.GetVueResource();
            var builder = new StringBuilder();
            Action<string, ResourceReader> add = (file, resourceLoder) => builder.Append(resourceLoder.LoadJavascript(file, !debugMode, !debugMode));
            Action<string> addComom = (file) => add(file, commonLoader);
            Action<string> addVersion = (file) => add(file, versionLoader);

            if (debugMode)
                addComom("hook");

            addVersion("vue");
            addComom("subscribeArray");
            addVersion("vueAdapter");
            addVersion("vueComandDirective");
            addComom("vueGlue");
            return builder.ToString();
        }

        private ResourceReader GetResourceReader(string path = "scripts")
        {
            return new ResourceReader(path, this);
        }
    }
}
