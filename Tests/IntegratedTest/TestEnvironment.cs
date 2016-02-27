using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedTest
{
    public class TestEnvironment
    {
        public Func<IWindowlessJavascriptEngine> WindowlessJavascriptEngineBuilder { get; set; }

        public Func<IWebView,IJavascriptFrameworkExtractor>  JavascriptFrameworkExtractorBuilder  { get; set; }

        public IDispatcher TestUIDispacther { get; set; }
    }
}
