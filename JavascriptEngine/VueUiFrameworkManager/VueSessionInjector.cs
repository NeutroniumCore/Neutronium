using MVVM.HTML.Core.JavascriptUIFramework;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Infra;
using System.IO;
using System;
using System.Text;

namespace VueUiFramework
{
    public class VueSessionInjector : IJavascriptUIFrameworkManager
    {
        public string FrameworkName => "vue.js 1.0.25";
        public string Name => "VueInjector";
        private string[] _DebugScript;
        private const string _ToogleDebug = "window.vueDebug();";

        public IJavascriptViewModelManager CreateManager(IWebView webView, IJavascriptObject listener) 
        {
            return new VueVmManager(webView, listener);
        }

        public string[] GetDebugScript()
        {
            if (_DebugScript != null)
                return _DebugScript;

            var loader = GetResourceReader();
            //var hook = loader.Load("hook.js");
            var almost = loader.Load("vuedebug.js");
            var updated = almost.Replace(@"build/devtools.js", GetFilePath("scripts/devtools.js"));
            var builder = new StringBuilder(updated);
            builder.AppendLine(_ToogleDebug);
            _DebugScript = new[] { builder.ToString()};
            //, hook };
            return _DebugScript;
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

        public string GetMainScript()
        {
            var loader = GetResourceReader();           
            return loader.Load ("hook.js", "vue.js","subscribeArray.min.js", "vueGlue.js");
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
