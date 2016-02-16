using System;
using System.IO;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

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

            using (var stream = GetType().Assembly.GetManifestResourceStream("MVVM.HTML.Core.Binding.Mapping.KnockoutInject.javascript.ko-view.min.js"))
            using (var reader = new StreamReader(stream))
            {
                _JavascriptDebugScript = reader.ReadToEnd();
            }
            return _JavascriptDebugScript;
        }

        public bool HasDebugScript()
        {
            return true;
        }
    }
}
