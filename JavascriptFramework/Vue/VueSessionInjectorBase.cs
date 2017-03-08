using System;
using System.IO;
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
        public string FrameworkName => _VueVersion.FrameworkName;
        public string Name => _VueVersion.Name;
        public string DebugToolbaRelativePath => _VueVersion.ToolBarPath;

        private string _DebugScript;
        private const string _ToogleDebug = "window.vueDebug();";
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

        private string GetDebugScript()
        {
            if (_DebugScript != null)
                return _DebugScript;

            var loader = GetResourceReader();
            var almost = loader.Load("vuedebug.js");
            var updated = almost.Replace(@"build/devtools.js", GetFilePath("scripts/devtools.js"));        
            var builder = new StringBuilder(updated);
            builder.Append(GetPathInjectorscript());

            return _DebugScript = builder.ToString();
        }

        private string GetPathInjectorscript()
        {
            return $"(function(){{window.__vue__backend__path__='{GetFilePath("scripts/backend.js")}';window.__vue__logo__path__='{GetFilePath("resource/logo.png")}';}})();";
        }

        private static string GetFilePath(string scriptPath)
        {
            var path = Path.GetDirectoryName( typeof(VueSessionInjectorBase).Assembly.Location);
            var fullPath = Path.Combine(path, scriptPath);
            return new Uri(fullPath).AbsoluteUri;
        }

        private string GetDebugToogleScript() 
        {
            return _ToogleDebug;
        }

        public void DebugVm(Action<string> runJavascript, Action<string, Action<IWebView, IWebView>> openNewWindow)
        {
            //var javascriptDebugScript = GetDebugScript();
            //runJavascript(javascriptDebugScript);
            //runJavascript(GetDebugToogleScript());
            openNewWindow(@"DebugTools\Window\index.html", RegisterDebugWindowHook);
        }

        private void RegisterDebugWindowHook(IWebView current, IWebView debugWebView) 
        {
            _WebViewCommunication.Listen(current, debugWebView);
            _WebViewCommunication.Listen(debugWebView, current);
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

        public bool HasDebugScript() => true;

        private ResourceReader GetResourceReader()
        {
            return new ResourceReader("scripts", this);
        }
    }
}
