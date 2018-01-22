using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreCollection.Extensions;
using Neutronium.Core;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.JavascriptFramework.mobx 
{
    public class MobxFrameworkManager : IJavascriptFrameworkManager 
    {
        public string FrameworkName => "mobx";

        public string FrameworkVersion => "v3.4.1";

        public string Name => "MobxFrameworkManager";

        public bool IsMappingObject => false;

        public DebugToolsUI DebugToolsUI => null;

        public IJavascriptViewModelManager CreateManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger, bool debugMode) 
        {
            return new MobxViewModelManager(webView, listener, logger);
        }

        public void DebugVm(Action<string> runJavascript, Action<string, int, int, Func<IWebView, IWebView, IDisposable>> openNewWindow) 
        {
            throw new NotImplementedException();
        }

        public string GetMainScript(bool debugContext)
        {
            var resourceLoader = GetResourceReader();
            var builder = _JavascriptFiles.Aggregate(new StringBuilder(), 
                            (sb, fn) => sb.Append(resourceLoader.Load(fn)).AppendLine());

            return builder.ToString();
        }

        private static readonly string[] _JavascriptFiles = 
        {
            "node_modules.mobx.lib.mobx.umd.min.js",
            "dist.mobxManager.js"
        };

        private ResourceReader GetResourceReader() 
        {
            return new ResourceReader("script", this);
        }
    }
}
