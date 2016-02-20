using System.IO;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Infra;

namespace MVVM.HTML.Core.Binding.Mapping
{
    public class KnockoutSessionInjectorFactory : IJavascriptSessionInjectorFactory
    {
        private string _JavascriptDebugScript;
        public IJavascriptSessionInjector CreateInjector(IWebView webView, IJavascriptChangesObserver javascriptObserver)
        {
            return new KnockoutSessionInjector(webView, javascriptObserver);
        }

        public string GetDebugScript()
        {
            if (_JavascriptDebugScript != null)
                return _JavascriptDebugScript;

            _JavascriptDebugScript = new ResourceReader("MVVM.HTML.Core.Binding.Mapping.KnockoutInject.javascript",this)
                                                  .Load("ko-view.min.js");
            return _JavascriptDebugScript ;
        }

        public bool HasDebugScript()
        {
            return true;
        }
    }
}
