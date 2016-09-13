using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Tests.Infra.JavascriptFrameworkTesterHelper;

namespace KnockoutFramework.Test.IntegratedInfra
{
    public class KnockoutExtractor : IJavascriptFrameworkExtractor
    {
        private readonly IWebView _WebView;
        public KnockoutExtractor(IWebView webView)
        {
            _WebView = webView;
        }

        public IJavascriptObject GetAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView));
        }

        public IJavascriptObject GetCollectionAttribute(IJavascriptObject value, string attibutename)
        {
            var almost = GetAttribute(value, attibutename);
            return almost.ExecuteFunction(_WebView);
        }

        public string GetStringAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView).GetStringValue());
        }

        public int GetIntAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView).GetIntValue());
        }

        public double GetDoubleAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView).GetDoubleValue());
        }

        public bool GetBoolAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView).GetBoolValue());
        }

        public void SetAttribute(IJavascriptObject father, string attibutename, IJavascriptObject value)
        {
            _WebView.Evaluate(() => father.Invoke(attibutename, _WebView, value));
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
