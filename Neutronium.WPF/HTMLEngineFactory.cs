using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.Log;
using MoreCollection.Extensions;
using Neutronium.Core.Infra;

namespace Neutronium.WPF
{
    /// <summary>
    /// IHTMLEngineFactory implementation
    /// </summary>
    public class HTMLEngineFactory : IHTMLEngineFactory 
    {
        private IWebSessionLogger _WebSessionLogger;
        private IJavascriptFrameworkManager _DefaultJavascriptFrameworkManager;
        private readonly IDictionary<string, IWPFWebWindowFactory> _Engines = new Dictionary<string, IWPFWebWindowFactory>();
        private readonly IDictionary<string, IJavascriptFrameworkManager> _JavascriptFrameworks = new Dictionary<string, IJavascriptFrameworkManager>();
    
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
            _WebSessionLogger = new BasicLogger();
        }

        public IWPFWebWindowFactory ResolveJavaScriptEngine(string engineName)
        {
            if (_Engines.Count != 1) 
                return _Engines.GetOrDefault(engineName);
            
            var res = _Engines.First().Value;
            if (!string.IsNullOrEmpty(engineName) && (res.Name != engineName))
            {
                _WebSessionLogger.Info(() => $"Name mismatch in IWPFWebWindowFactory resolution {engineName} vs {res.Name}");
            }
            return res;
        }

        public bool HasJavaScriptEngine(string engineName) 
        {
            return _JavascriptFrameworks.ContainsKey(engineName);
        }

        public void RegisterHTMLEngine(IWPFWebWindowFactory wpfWebWindowFactory)
        {
            _Engines[wpfWebWindowFactory.Name] = wpfWebWindowFactory;
        }

        private IJavascriptFrameworkManager PrivateResolveJavaScriptFramework(string frameworkName) 
        {
            if (_JavascriptFrameworks.Count != 1) 
                return _JavascriptFrameworks.GetOrDefault(frameworkName, _DefaultJavascriptFrameworkManager);

            var res = _JavascriptFrameworks.First().Value;
            if (!string.IsNullOrEmpty(frameworkName) && (res.Name != frameworkName)) 
            {
                _WebSessionLogger.Info(() => $"Name mismatch in IJavascriptUIFrameworkManager resolution {frameworkName} vs {res.Name}");
            }
            return res;
        }

        public IJavascriptFrameworkManager ResolveJavaScriptFramework(string frameworkName) 
        {
            var res = PrivateResolveJavaScriptFramework(frameworkName);
            _WebSessionLogger.Debug($"Resolving Javascript framekork using: {res?.Name}");
            return res;
        }

        public void RegisterJavaScriptFramework(IJavascriptFrameworkManager javascriptFrameworkManager)
        {
            _JavascriptFrameworks[javascriptFrameworkManager.Name]= javascriptFrameworkManager;
        }

        public void RegisterJavaScriptFrameworkAsDefault(IJavascriptFrameworkManager javascriptFrameworkManager) 
        {
            RegisterJavaScriptFramework(javascriptFrameworkManager);
            _DefaultJavascriptFrameworkManager = javascriptFrameworkManager;
        }

        public IWebSessionLogger WebSessionLogger 
        {
            get => _WebSessionLogger;
            set
            {
                _WebSessionLogger = value?? new NullLogger();
                OnEngines(fact => fact.WebSessionLogger = _WebSessionLogger);
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

        public HTMLWindowInfo ResolveToolbar() 
        {
            return Resolve(fr => fr.DebugToolsUI?.DebugToolbar);        
        }

        public HTMLWindowInfo ResolveAboutScreen() 
        {
            return Resolve(fr => fr.DebugToolsUI?.About);
        }

        public HTMLWindowInfo Resolve(Func<IJavascriptFrameworkManager, WindowInformation> windowInfoGetter) {
            return _JavascriptFrameworks.Values.Select(framework => new  { Window = windowInfoGetter(framework) , Framework= framework })
                                        .Where(info => info.Window!= null)
                                        .Select(info => new HTMLWindowInfo {
                                            Height =info.Window.Height,
                                            Width = info.Window.Width,
                                            AbsolutePath = GetPath(info.Framework, info.Window),
                                            Framework = info.Framework })
                                        .FirstOrDefault();
        }

        private static string GetPath(IJavascriptFrameworkManager javascriptFrameworkManager,  WindowInformation windowInfo)
        {
            return  $"{javascriptFrameworkManager.GetType().Assembly.GetPath()}\\{windowInfo.RelativePath}";
        }
    }
}
