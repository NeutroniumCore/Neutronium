using System;
using System.Linq;
using System.Text;
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
        public DebugToolsUI DebugToolsUI => new DebugToolsUI(null, About);
        public bool IsSupportingVmDebug => false;

        private static WindowInformation About => new WindowInformation 
        {
            RelativePath = "DebugTools\\About\\index.html",
            Height = 640,
            Width = 310
        };

        public IJavascriptViewModelManager CreateManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger, bool debugMode) 
        {
            return new MobxViewModelManager(webView, listener, logger);
        }

        public void DebugVm(Action<string> runJavascript, Action<string, int, int, Func<IWebView, IWebView, IDisposable>> openNewWindow) 
        {
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
            "mobx.umd.min.js",
            "mobxManager.js"
        };

        private ResourceReader GetResourceReader() 
        {
            return new ResourceReader("script.dist", this);
        }
    }
}
