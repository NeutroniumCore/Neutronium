using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace HTML_WPF.Component
{
    /// <summary>
    /// IHTMLEngineFactory implementation
    /// </summary>
    public class HTMLEngineFactory : IHTMLEngineFactory
    {
        private IDictionary<string, IWPFWebWindowFactory> _Engines = new Dictionary<string, IWPFWebWindowFactory>();
        private IDictionary<string, IJavascriptUIFrameworkManager> _JavascriptFrameworks = new Dictionary<string, IJavascriptUIFrameworkManager>();

        /// <summary>
        /// Access the singleton IHTMLEngineFactory
        /// </summary>
        public static IHTMLEngineFactory Engine
        {
            get; internal set;
        }

        static HTMLEngineFactory()
        {
            Engine = new HTMLEngineFactory();
        }

        public IWPFWebWindowFactory ResolveJavaScriptEngine(string EngineName)
        {
            if (_Engines.Count == 1)
            {
                var res = _Engines.First().Value;
                if (!string.IsNullOrEmpty(EngineName) && (res.Name != EngineName))
                {
                    Trace.WriteLine(string.Format("Name mismatch in IWPFWebWindowFactory resolution {0} vs {1}", EngineName, res.Name));
                }
                return res;
            }

            return _Engines.GetOrDefault(EngineName);
        }

        public void Register(IWPFWebWindowFactory wpfWebWindowFactory)
        {
            _Engines[wpfWebWindowFactory.Name] = wpfWebWindowFactory;
        }

        public IJavascriptUIFrameworkManager ResolveJavaScriptFramework(string frameworkName)
        {
            if (_JavascriptFrameworks.Count == 1)
            {
                var res = _JavascriptFrameworks.First().Value;
                if (!string.IsNullOrEmpty(frameworkName) && (res.Name != frameworkName))
                {
                    Trace.WriteLine(string.Format("Name mismatch in IJavascriptUIFrameworkManager resolution {0} vs {1}", frameworkName, res.Name));
                }
                return res;
            }

            return _JavascriptFrameworks.GetOrDefault(frameworkName);
        }

        public void Register(IJavascriptUIFrameworkManager javascriptUiFrameworkManager)
        {
            _JavascriptFrameworks[javascriptUiFrameworkManager.Name]= javascriptUiFrameworkManager;
        }

        public void Dispose()
        {
            _Engines.Values.ForEach(fact => fact.Dispose());
            _Engines.Clear();
        }
    }
}
