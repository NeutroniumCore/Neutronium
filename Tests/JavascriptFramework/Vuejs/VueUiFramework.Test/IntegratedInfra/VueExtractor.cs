using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Tests.Infra.JavascriptFrameworkTesterHelper;

namespace VueFramework.Test.IntegratedInfra
{
    public class VueExtractor : IJavascriptFrameworkExtractor
    {
        private readonly IWebView _WebView;
        public VueExtractor(IWebView webView)
        {
            _WebView = webView;
        }

        public IJavascriptObject GetAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.GetValue(attibutename));
        }

        public bool GetBoolAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.GetValue(attibutename).GetBoolValue());
        }

        public IJavascriptObject GetCollectionAttribute(IJavascriptObject value, string attibutename)
        {
            return GetAttribute(value, attibutename);
        }

        public double GetDoubleAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.GetValue(attibutename).GetDoubleValue());
        }

        public int GetIntAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.GetValue(attibutename).GetIntValue());
        }

        public string GetStringAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.GetValue(attibutename).GetStringValue());
        }

        public void SetAttribute(IJavascriptObject father, string attibutename, IJavascriptObject value)
        {
            _WebView.Run(() => father.SetValue(attibutename, value));
        }

        public IJavascriptObject GetRootViewModel()
        {
            return _WebView.Evaluate(() => UnSafeGetRootViewModel());
        }

        private IJavascriptObject UnSafeGetRootViewModel()
        {
            var window = _WebView.GetGlobal();
            return window.GetValue("vm");
        }
    }
}
