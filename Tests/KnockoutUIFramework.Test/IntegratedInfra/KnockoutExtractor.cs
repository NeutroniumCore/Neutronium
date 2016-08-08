using IntegratedTest.Infra.Windowless;
using IntegratedTest.JavascriptUIFramework;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace KnockoutUIFramework.Test.IntegratedInfra
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
    }
}
