using MVVM.HTML.Core.JavascriptUIFramework;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Infra;

namespace VueUiFramework
{
    public class VueSessionInjector : IJavascriptUIFrameworkManager
    {
        public string FrameworkName => "vue.js 1.0.25";
        public string Name => "VueInjector";

        public IJavascriptViewModelManager CreateManager(IWebView webView, IJavascriptObject listener) 
        {
            return new VueVmManager(webView, listener);
        }

        public string GetDebugScript()
        {
            return null;
        }

        public string GetDebugToogleScript() 
        {
            return null;
        }

        public string GetMainScript()
        {
            var loader = GetResourceReader();           
            return loader.Load("vue.js", "subscribeArray.min.js", "vueGlue.js");
        }

        public bool HasDebugScript()
        {
            return false;
        }

        private ResourceReader GetResourceReader()
        {
            return new ResourceReader("VueUiFramework.scripts", this);
        }
    }
}
