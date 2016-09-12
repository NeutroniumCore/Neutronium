using System.IO;
using System;
using System.Text;
using System.Collections.Generic;
using Neutronium.Core;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptUIFramework;

namespace VueUiFramework
{
    public class VueSessionInjector : IJavascriptUiFrameworkManager
    {
        public string FrameworkName => "vue.js 1.0.25";
        public string Name => "VueInjector";
        private string _DebugScript;
        private const string _ToogleDebug = "window.vueDebug();";

        public IJavascriptViewModelManager CreateManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger) 
        {
            return new VueVmManager(webView, listener, logger);
        }

        public string GetDebugScript()
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
            var path = Path.GetDirectoryName( typeof(VueSessionInjector).Assembly.Location);
            var fullPath = Path.Combine(path, scriptPath);
            return new Uri(fullPath).AbsoluteUri;
        }

        public string GetDebugToogleScript() 
        {
            return _ToogleDebug;
        }

        public string GetMainScript(bool debugContext)
        {
            var loader = GetResourceReader();
            return loader.Load(GetJavascriptSource(debugContext));
        }

        private static IEnumerable<string> GetJavascriptSource(bool debugMode)
        {
            if (debugMode)
                yield return "hook.js";
            yield return "vue.js";
            yield return "subscribeArray.min.js";
            yield return "vueComandDirective.js";
            yield return "vueGlue.js";
        }

        public bool HasDebugScript()
        {
            return true;
        }

        private ResourceReader GetResourceReader()
        {
            return new ResourceReader("VueUiFramework.scripts", this);
        }
    }
}
