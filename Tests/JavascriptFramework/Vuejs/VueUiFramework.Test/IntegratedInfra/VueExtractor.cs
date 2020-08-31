using System.Threading.Tasks;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Tests.Infra.JavascriptFrameworkTesterHelper;

namespace VueFramework.Test.IntegratedInfra
{
    public class VueExtractor : IJavascriptFrameworkExtractor
    {
        private readonly IWebView _WebView;

        public bool SupportDynamicBinding => true;

        public VueExtractor(IWebView webView)
        {
            _WebView = webView;
        }

        public IJavascriptObject GetAttribute(IJavascriptObject value, string attributeName)
        {
            return _WebView.Evaluate(() => value.GetValue(attributeName));
        }

        public bool GetBoolAttribute(IJavascriptObject value, string attributeName)
        {
            return _WebView.Evaluate(() => value.GetValue(attributeName).GetBoolValue());
        }

        public IJavascriptObject GetCollectionAttribute(IJavascriptObject value, string attributeName)
        {
            return GetAttribute(value, attributeName);
        }

        public double GetDoubleAttribute(IJavascriptObject value, string attributeName)
        {
            return _WebView.Evaluate(() => value.GetValue(attributeName).GetDoubleValue());
        }

        public int GetIntAttribute(IJavascriptObject value, string attributeName)
        {
            return _WebView.Evaluate(() => value.GetValue(attributeName).GetIntValue());
        }

        public void AddAttribute(IJavascriptObject father, string attributeName, IJavascriptObject value)
        {
            var vue = _WebView.GetGlobal().GetValue("Vue");
            vue.InvokeNoResult("set", _WebView, father, _WebView.Factory.CreateString(attributeName), value);
        }

        public string GetStringAttribute(IJavascriptObject value, string attributeName)
        {
            return _WebView.Evaluate(() => value.GetValue(attributeName).GetStringValue());
        }

        public void SetAttribute(IJavascriptObject father, string attributeName, IJavascriptObject value)
        {
            _WebView.Run(() => father.SetValue(attributeName, value));
        }

        public Task SetAttributeAsync(IJavascriptObject father, string attributeName, IJavascriptObject value)
        {
            return _WebView.RunAsync(() => father.SetValue(attributeName, value));
        }

        public IJavascriptObject GetRootViewModel()
        {
            return _WebView.Evaluate(UnSafeGetRootViewModel);
        }

        private IJavascriptObject UnSafeGetRootViewModel()
        {
            var window = _WebView.GetGlobal();
            return window.GetValue("vm");
        }
    }
}
