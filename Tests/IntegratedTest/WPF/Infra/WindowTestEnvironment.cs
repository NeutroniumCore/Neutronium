using HTML_WPF.Component;
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
    public class WindowTestEnvironment : IDisposable
    {
        public IWPFWebWindowFactory WPFWebWindowFactory { get; set; }

        public IJavascriptUIFrameworkManager FrameworkManager { get; set; }

        public void Register()
        {
            var engine = HTMLEngineFactory.Engine;
            engine.Register(WPFWebWindowFactory);
            engine.Register(FrameworkManager);
        }

        public void Dispose()
        {
            HTMLEngineFactory.Engine.Dispose();
        }
    }
}
