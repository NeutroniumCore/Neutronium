using System.Threading.Tasks;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Tests.Infra.JavascriptFrameworkTesterHelper;

namespace KnockoutFramework.Test.IntegratedInfra
{
    public class KnockoutExtractor : IJavascriptFrameworkExtractor
    {
        private readonly IWebView _WebView;

        public bool SupportDynamicBinding => false;

        public KnockoutExtractor(IWebView webView)
        {
            _WebView = webView;
        }

        public IJavascriptObject GetAttribute(IJavascriptObject value, string attributeName)
        {
            return _WebView.Evaluate(() => value.Invoke(attributeName, _WebView));
        }

        public Task<IJavascriptObject> GetAttributeAsync(IJavascriptObject value, string attributeName)
        {
            return _WebView.EvaluateAsync(() => value.Invoke(attributeName, _WebView));
        }

        public IJavascriptObject GetCollectionAttribute(IJavascriptObject value, string attributeName)
        {
            var almost = GetAttribute(value, attributeName);
            return almost.ExecuteFunction(_WebView);
        }

        public async Task<IJavascriptObject> GetCollectionAttributeAsync(IJavascriptObject value, string attributeName)
        {
            var almost = await GetAttributeAsync(value, attributeName);
            return almost.ExecuteFunction(_WebView);
        }

        public void AddAttribute(IJavascriptObject father, string attributeName, IJavascriptObject value)
        {
            throw new System.NotImplementedException();
        }

        public string GetStringAttribute(IJavascriptObject value, string attributeName)
        {
            return _WebView.Evaluate(() => value.Invoke(attributeName, _WebView).GetStringValue());
        }

        public int GetIntAttribute(IJavascriptObject value, string attributeName)
        {
            return _WebView.Evaluate(() => value.Invoke(attributeName, _WebView).GetIntValue());
        }

        public double GetDoubleAttribute(IJavascriptObject value, string attributeName)
        {
            return _WebView.Evaluate(() => value.Invoke(attributeName, _WebView).GetDoubleValue());
        }

        public bool GetBoolAttribute(IJavascriptObject value, string attributeName)
        {
            return _WebView.Evaluate(() => value.Invoke(attributeName, _WebView).GetBoolValue());
        }

        public Task SetAttributeAsync(IJavascriptObject father, string attributeName, IJavascriptObject value)
        {
            return _WebView.EvaluateAsync(() => father.Invoke(attributeName, _WebView, value));
        }

        public IJavascriptObject GetRootViewModel()
        {
            return _WebView.Evaluate(() => UnSafeGetRootViewModel());
        }

        private IJavascriptObject UnSafeGetRootViewModel()
        {
            var window = _WebView.GetGlobal();
            var ko = window.GetValue("ko");
            var document = window.GetValue("document");
            var body = document.GetValue("body");
            return ko.Invoke("dataFor", _WebView, body);
        }
    }
}
