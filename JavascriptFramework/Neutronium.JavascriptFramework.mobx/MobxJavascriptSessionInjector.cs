using System;
using System.Threading.Tasks;
using Neutronium.Core;
using Neutronium.Core.Extension;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.JavascriptFramework.mobx 
{
    public class MobxJavascriptSessionInjector : IJavascriptSessionInjector 
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Listener;
        private readonly IWebSessionLogger _Logger;
        private readonly Lazy<IJavascriptObject> _MobxHelperLazy;

        public MobxJavascriptSessionInjector(IWebView webView, Lazy<IJavascriptObject> helper, IJavascriptObject listener, IWebSessionLogger logger) 
        {
            _WebView = webView;
            _Listener = listener;
            _Logger = logger;
            _MobxHelperLazy = helper;
        }

        public IJavascriptObject Inject(IJavascriptObject rawObject, IJavascriptObjectMapper mapper) 
        {
            return rawObject;
        }

        public Task RegisterMainViewModel(IJavascriptObject jsObject) 
        {
            var mobxHelper = _MobxHelperLazy.Value;
            var taskProvider = mobxHelper.GetValue("done").TransformPromiseToTask(_WebView);
            mobxHelper.Invoke("register", _WebView, jsObject, _Listener);
            return taskProvider.Task;
        }
    }
}
