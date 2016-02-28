using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using MVVM.HTML.Core.JavascriptUIFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedTest
{
    public class WindowlessTestEnvironment
    {
        public Func<IJavascriptUIFrameworkManager, IWindowlessJavascriptEngine> WindowlessJavascriptEngineBuilder { get; set; }

        public IJavascriptUIFrameworkManager FrameworkManager { get; set; }

        public Func<IWebView,IJavascriptFrameworkExtractor> JavascriptFrameworkExtractorBuilder  { get; set; }

        public IDispatcher TestUIDispacther { get; set; }

        public IWindowlessJavascriptEngine Build()
        {
            return WindowlessJavascriptEngineBuilder(FrameworkManager);
        }
    }
}
