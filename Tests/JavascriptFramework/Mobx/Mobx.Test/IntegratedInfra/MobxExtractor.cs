using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Tests.Infra.JavascriptFrameworkTesterHelper;

namespace Mobx.Test.IntegratedInfra
{
    public class MobxExtractor : IJavascriptFrameworkExtractor
    {
        private readonly IWebView _WebView;
        public MobxExtractor(IWebView webView)
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
            var almost = GetAttribute(value, attibutename);
            return almost.Invoke("slice",_WebView);
        }

        public double GetDoubleAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.GetValue(attibutename).GetDoubleValue());
        }

        public int GetIntAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.GetValue(attibutename).GetIntValue());
        }

        public void AddAttribute(IJavascriptObject father, string attibutename, IJavascriptObject value)
        {
            _WebView.Run(() =>
            {
                var mobx = _WebView.GetGlobal().GetValue("mobx");
                var helper = _WebView.Factory.CreateObject(true);
                helper.SetValue(attibutename, value);
                mobx.InvokeNoResult("extendObservable", _WebView, father, helper);
            });
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
            return _WebView.Evaluate(UnSafeGetRootViewModel);
        }

        private IJavascriptObject UnSafeGetRootViewModel()
        {
            var window = _WebView.GetGlobal();
            return window.GetValue("_vm");
        }
    }
}
