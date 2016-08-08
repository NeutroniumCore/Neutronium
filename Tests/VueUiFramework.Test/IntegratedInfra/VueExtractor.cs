using IntegratedTest.Infra.Windowless;
using IntegratedTest.JavascriptUIFramework;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace VueUiFramework.Test.IntegratedInfra
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
    }
}
