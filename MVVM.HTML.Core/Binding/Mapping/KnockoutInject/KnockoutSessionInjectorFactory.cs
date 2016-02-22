using System.IO;
using System.Linq;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Infra;
using System.Collections.Generic;
using System;

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

            _JavascriptDebugScript = GetResourceReader().Load("ko-view.min.js");
            return _JavascriptDebugScript ;
        }

        public bool HasDebugScript()
        {
            return true;
        }

        public void ExcecuteFirst(Action<string> executeSript)
        {
            var resourceLoader = GetResourceReader();
            JavascriptSource.Select(resourceLoader.Load).ForEach(executeSript);
        }

        private static IEnumerable<string> JavascriptSource
        {
            get
            {
                yield return "knockout.js";
                yield return "knockout-delegatedEvents.min.js";
                yield return "promise.min.js";
                yield return "Ko_Extension.min.js";
            }
        }

        private ResourceReader GetResourceReader()
        {
            return new ResourceReader("MVVM.HTML.Core.Binding.Mapping.KnockoutInject.scripts", this);
        }
    }
}
