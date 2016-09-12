using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptUIFramework;

namespace HTML_WPF.Component
{
    /// <summary>
    /// IHTMLEngineFactory implementation
    /// </summary>
    public class HTMLEngineFactory : IHTMLEngineFactory 
    {
        private IWebSessionLogger _webSessionLogger;
        private readonly IDictionary<string, IWPFWebWindowFactory> _Engines = new Dictionary<string, IWPFWebWindowFactory>();
        private readonly IDictionary<string, IJavascriptUiFrameworkManager> _JavascriptFrameworks = new Dictionary<string, IJavascriptUiFrameworkManager>();

        /// <summary>
        /// Access the singleton IHTMLEngineFactory
        /// </summary>
        public static IHTMLEngineFactory Engine { get; internal set;  }

        static HTMLEngineFactory()
        {
            Engine = new HTMLEngineFactory();
        }

        public HTMLEngineFactory() 
        {
            _webSessionLogger = new BasicLogger();
        }

        public IWPFWebWindowFactory ResolveJavaScriptEngine(string engineName)
        {
            if (_Engines.Count != 1) 
                return _Engines.GetOrDefault(engineName);
            
            var res = _Engines.First().Value;
            if (!string.IsNullOrEmpty(engineName) && (res.Name != engineName))
            {
                _webSessionLogger.Info(() => $"Name mismatch in IWPFWebWindowFactory resolution {engineName} vs {res.Name}");
            }
            return res;
        }

        public void RegisterHTMLEngine(IWPFWebWindowFactory wpfWebWindowFactory)
        {
            _Engines[wpfWebWindowFactory.Name] = wpfWebWindowFactory;
            wpfWebWindowFactory.WebSessionLogger = _webSessionLogger;
        }

        public IJavascriptUiFrameworkManager ResolveJavaScriptFramework(string frameworkName)
        {
            if (_JavascriptFrameworks.Count == 1)
            {
                var res = _JavascriptFrameworks.First().Value;
                if (!string.IsNullOrEmpty(frameworkName) && (res.Name != frameworkName))
                {
                    _webSessionLogger.Info(() => $"Name mismatch in IJavascriptUIFrameworkManager resolution {frameworkName} vs {res.Name}");
                }
                return res;
            }

            return _JavascriptFrameworks.GetOrDefault(frameworkName);
        }

        public void RegisterJavaScriptFramework(IJavascriptUiFrameworkManager javascriptUiFrameworkManager)
        {
            _JavascriptFrameworks[javascriptUiFrameworkManager.Name]= javascriptUiFrameworkManager;
        }

        public IWebSessionLogger WebSessionLogger 
        {
            get { return _webSessionLogger; } 
            set
            {
                _webSessionLogger = value?? new NullLogger();
                OnEngines(fact => fact.WebSessionLogger = _webSessionLogger);
            }
        }

        private void OnEngines(Action<IWPFWebWindowFactory> action) 
        {
            _Engines.Values.ForEach(action);
        }

        public void Dispose()
        {
            OnEngines(fact => fact.Dispose());
            _Engines.Clear();
        }
    }
}
