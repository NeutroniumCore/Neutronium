using System.Collections.Generic;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace KnockoutUIFramework
{
    public class KnockoutUiFrameworkManager : IJavascriptUIFrameworkManager
    {
        private string _JavascriptDebugScript;
        private string _MainScript;

        public string FrameworkName => "knockout.js 3.3.0";
        public string Name => "KnockoutInjector";

        public IJavascriptViewModelManager CreateManager(IWebView webView, IJavascriptObject listener) 
        {
            return new KnockoutUiVmManager(webView, listener);
        }

        public string GetDebugScript()
        {
            if (_JavascriptDebugScript != null)
                return _JavascriptDebugScript;

            _JavascriptDebugScript = GetResourceReader().Load("ko-view.min.js");
            return _JavascriptDebugScript ;
        }

        public string GetDebugToogleScript()
        {
            return "ko.dodebug();";
        }

        public bool HasDebugScript()
        {
            return true;
        }

        public string GetMainScript()
        {
            if (_MainScript != null)
                return _MainScript;

            var resourceLoader = GetResourceReader();
            return resourceLoader.Load(JavascriptSource);
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
            return new ResourceReader("KnockoutUIFramework.scripts", this);
        }
    }
}
