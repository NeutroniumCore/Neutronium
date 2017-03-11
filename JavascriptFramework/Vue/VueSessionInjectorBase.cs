using System;
using System.Text;
using Neutronium.Core;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.JavascriptFramework.Vue.Communication;
using System.Diagnostics;

namespace Neutronium.JavascriptFramework.Vue
{
    public class VueSessionInjectorBase : IJavascriptFrameworkManager
    {
        public string FrameworkName => _VueVersion.FrameworkName;
        public string Name => _VueVersion.Name;
        public string DebugToolbaRelativePath => _VueVersion.ToolBarPath;

        private readonly IVueVersion _VueVersion;
        private readonly IWebViewCommunication _WebViewCommunication;

        protected VueSessionInjectorBase(IVueVersion vueVersion)
        {
            _VueVersion = vueVersion;
            _WebViewCommunication = new WebViewCommunication();
        }

        public IJavascriptViewModelManager CreateManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger, bool debugMode) 
        {
            return new VueVmManager(webView, listener, debugMode ? _WebViewCommunication : null, logger);
        }

        public void DebugVm(Action<string> runJavascript, Action<string, Action<IWebView, IWebView>> openNewWindow)
        {
            openNewWindow(@"DebugTools\Window\index.html", RegisterDebugWindowHook);
        }

        private void RegisterDebugWindowHook(IWebView current, IWebView debugWebView) 
        {
            _WebViewCommunication.ExecuteCodeOnAllEvents(current, debugWebView, PostMessage);
            _WebViewCommunication.ExecuteCodeOnAllEvents(debugWebView, current, PostMessage);
            _WebViewCommunication.Subscribe(debugWebView, "inject", _ => InjectBackend(current));
        }

        private static string PostMessage(string channel, string message)
        {
            var code = $"window.__listener__.emitter.emit({channel},{message});";
            Trace.WriteLine(code);
            return code;
        }

        private void InjectBackend(IWebView current)
        {
            var loader = new ResourceReader("DebugTools.Window.dist", this);
            var data = loader.Load("backend.js");
            data += ";window.__listener__.postMessage('injectDone', '');";
            current.ExecuteJavaScript(data);
        }

        public string GetMainScript(bool debugMode)
        {
            var commonLoader = GetResourceReader();
            var versionLoader = _VueVersion.GetVueResource();
            var builder = new StringBuilder();
            Action<string, ResourceReader> add = (file, resourceLoder) => builder.Append(resourceLoder.LoadJavascript(file, !debugMode, !debugMode));
            Action<string> addComom = (file) => add(file, commonLoader);
            Action<string> addVersion = (file) => add(file, versionLoader);

            if (debugMode)
                addComom("hook");

            addVersion("vue");
            addComom("subscribeArray");
            addVersion("vueAdapter");
            addVersion("vueComandDirective");
            addComom("vueGlue");
            return builder.ToString();
        }

        public bool HasDebugScript() => true;

        private ResourceReader GetResourceReader()
        {
            return new ResourceReader("scripts", this);
        }
    }
}
