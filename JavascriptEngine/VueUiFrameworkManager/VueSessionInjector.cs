using MVVM.HTML.Core.JavascriptUIFramework;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Infra;

namespace VueUiFramework
{
    public class VueSessionInjector : IJavascriptUIFrameworkManager
    {
        public string FrameworkName { get { return "vue.js 1.0.25"; } }

        public string Name { get { return "VueInjector"; } }

        public VueSessionInjector()
        {
        }

        public IJavascriptSessionInjector CreateInjector(IWebView webView, IJavascriptChangesObserver javascriptObserver)
        {
            return new VueJavascriptSessionInjector(webView, javascriptObserver);
        }

        public IJavascriptViewModelUpdater CreateViewModelUpdater(IWebView webView)
        {
            return new VueJavascriptViewModelUpdater(webView);
        }

        public string GetDebugScript()
        {
            return null;
        }

        public string GetMainScript()
        {
            var loader = GetResourceReader();           
            return loader.Load("vue.js", "vueGlue.js");
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
