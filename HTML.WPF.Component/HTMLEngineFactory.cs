using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;
using MVVM.HTML.Core.Navigation;

namespace HTML_WPF.Component
{
    /// <summary>
    /// IHTMLEngineFactory implementation
    /// </summary>
    public class HTMLEngineFactory : IHTMLEngineFactory 
    {
        private IWebSessionWatcher _WebSessionWatcher;
        private readonly IDictionary<string, IWPFWebWindowFactory> _Engines = new Dictionary<string, IWPFWebWindowFactory>();
        private readonly IDictionary<string, IJavascriptUIFrameworkManager> _JavascriptFrameworks = new Dictionary<string, IJavascriptUIFrameworkManager>();

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

        public HTMLEngineFactory() 
        {
            _WebSessionWatcher = new NullWatcher();
        }

        public IWPFWebWindowFactory ResolveJavaScriptEngine(string engineName)
        {
            if (_Engines.Count != 1) 
                return _Engines.GetOrDefault(engineName);
            
            var res = _Engines.First().Value;
            if (!string.IsNullOrEmpty(engineName) && (res.Name != engineName))
            {
                Trace.WriteLine($"Name mismatch in IWPFWebWindowFactory resolution {engineName} vs {res.Name}");
            }
            return res;
        }

        public void RegisterHTMLEngine(IWPFWebWindowFactory wpfWebWindowFactory)
        {
            _Engines[wpfWebWindowFactory.Name] = wpfWebWindowFactory;
            if (_WebSessionWatcher != null)
                wpfWebWindowFactory.WebSessionWatcher = _WebSessionWatcher;
        }

        public IJavascriptUIFrameworkManager ResolveJavaScriptFramework(string frameworkName)
        {
            if (_JavascriptFrameworks.Count == 1)
            {
                var res = _JavascriptFrameworks.First().Value;
                if (!string.IsNullOrEmpty(frameworkName) && (res.Name != frameworkName))
                {
                    Trace.WriteLine($"Name mismatch in IJavascriptUIFrameworkManager resolution {frameworkName} vs {res.Name}");
                }
                return res;
            }

            return _JavascriptFrameworks.GetOrDefault(frameworkName);
        }

        public void RegisterJavaScriptFramework(IJavascriptUIFrameworkManager javascriptUiFrameworkManager)
        {
            _JavascriptFrameworks[javascriptUiFrameworkManager.Name]= javascriptUiFrameworkManager;
        }

        public IWebSessionWatcher WebSessionWatcher 
        {
            get { return _WebSessionWatcher; } 
            set
            {
                _WebSessionWatcher = value;
                OnEngines(fact => fact.WebSessionWatcher = _WebSessionWatcher);
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
