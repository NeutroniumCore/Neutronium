using System;
using System.Text;
using Neutronium.Core;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.JavascriptFramework.Vue.Communication;

namespace Neutronium.JavascriptFramework.Vue
{
    public abstract class VueSessionInjectorBase : IJavascriptFrameworkManager
    {
        public string FrameworkName => "Vue.js";
        public string Name => _VueVersion.Name;
        public string FrameworkVersion => _VueVersion.FrameworkNameVersion;
        public DebugToolsUI DebugToolsUI => _VueVersion.DebugToolsUI;
        public bool IsMappingObject => false;
        public bool RunTimeOnly { get; set; } = false;
        public bool IsSupportingVmDebug => true;

        private readonly VueVersion _VueVersion;
        private readonly IWebViewCommunication _WebViewCommunication;
        private string VueFile => (_VueVersion.SupportRuntime && RunTimeOnly) ? "vue.runtime" : "vue";

        internal VueSessionInjectorBase(VueVersion vueVersion)
        {
            _VueVersion = vueVersion;
            _WebViewCommunication = new WebViewCommunication();
        }

        public IJavascriptViewModelManager CreateManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger, bool debugMode)
        {
            return new VueVmManager(webView, listener, debugMode ? _WebViewCommunication : null, logger);
        }

        public void DebugVm(IDebugFacility debugHelper)
        {
            debugHelper.OpenNewWindow(@"DebugTools\Window\index.html", 800, 700, RegisterDebugWindowHook);
        }

        private IDisposable RegisterDebugWindowHook(IWebView current, IWebView debugWebView)
        {
            var connection1 = _WebViewCommunication.Connect(current, debugWebView);
            var connection2 = _WebViewCommunication.Subscribe(debugWebView, "main:inject", _ => InjectBackend(current));
            var disconnect = new DisposableAction(() => _WebViewCommunication.Disconnect(debugWebView));
            return new ComposedDisposable(connection1, connection2, disconnect);
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
            void Add(string file, ResourceReader resourceLoader) => builder.Append(Load(resourceLoader, file, debugMode));
            void AddComom(string file) => Add(file, commonLoader);
            void AddVersion(string file) => Add(file, versionLoader);

            if (debugMode)
                AddComom("hook");

            AddVersion(VueFile);
            AddComom("subscribeArray");
            AddVersion("vueAdapter");
            AddVersion("vueComandDirective");
            builder.Append(Load(commonLoader, "vueGlue", debugMode).Replace("__debugMode__", debugMode.ToString().ToLower()));
            return builder.ToString();
        }

        private static string Load(ResourceReader resourceReader, string name, bool debugMode)
        {
            return resourceReader.LoadJavascript(name, !debugMode, !debugMode);
        }

        private ResourceReader GetResourceReader(string path = "scripts")
        {
            return new ResourceReader(path, this);
        }
    }
}
