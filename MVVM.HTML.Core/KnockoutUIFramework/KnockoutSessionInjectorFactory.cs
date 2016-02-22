using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core.KnockoutUIFramework
{
    public class KnockoutSessionInjectorFactory : IJavascriptSessionInjectorFactory
    {
        private string _JavascriptDebugScript;
        private string _MainScript;

        public string FrameworkName { get { return "knockout.js 3.3.0"; } }
        public string Name { get { return "KnockoutInjector "; } }

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

        public string GetMainScript()
        {
            if (_MainScript != null)
                return _MainScript;

            var resourceLoader = GetResourceReader();
            var builder = new StringBuilder();
            JavascriptSource.Select(resourceLoader.Load).ForEach(s => builder.Append(s));
            return (_MainScript = builder.ToString());
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
            return new ResourceReader("MVVM.HTML.Core.KnockoutUIFramework.scripts", this);
        }
    }
}
