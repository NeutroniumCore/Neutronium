using System;
using System.Collections.Generic;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;
using MVVM.HTML.Core.Infra;

namespace KnockoutUIFramework
{
    internal class KnockoutViewModelUpdater : IJavascriptViewModelUpdater, IDisposable
    {
        private readonly IWebView _WebView;
        private readonly IDictionary<IJavascriptObject, IDictionary<string, IJavascriptObject>> _Silenters = 
                    new Dictionary<IJavascriptObject, IDictionary<string, IJavascriptObject>>();
 
        internal KnockoutViewModelUpdater(IWebView webView)
        {
            _WebView = webView;
        }

        public void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value)
        {
            var silenter = GetSilenter(father, propertyName);
            if (silenter != null)
            {
                Silent(silenter, value);
                return;
            }

            _WebView.RunAsync(() =>
            {
                silenter = GetOrCreateSilenter(father, propertyName);
                Silent(silenter, value);
            });
        }

        private IJavascriptObject GetSilenter(IJavascriptObject father, string propertyName)
        {
            var dic = _Silenters.GetOrDefault(father);
            return (dic == null) ? null : dic.GetOrDefault(propertyName);
        }

        private IJavascriptObject GetOrCreateSilenter(IJavascriptObject father, string propertyName)
        {
            var dic = _Silenters.FindOrCreateEntity(father, _ => new Dictionary<string, IJavascriptObject>());
            return dic.FindOrCreateEntity(propertyName, name => father.GetValue(name));
        }

        private void Silent(IJavascriptObject silenter, IJavascriptObject value)
        {
            silenter.Invoke("silent", _WebView, value);
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject added)
        {
            array.InvokeAsync("silentsplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number), added);
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number)
        {
            array.InvokeAsync("silentsplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number));
        }

        public void ClearAllCollection(IJavascriptObject array)
        {
            array.InvokeAsync("silentremoveAll", _WebView);
        }

        public void MoveCollectionItem(IJavascriptObject array, IJavascriptObject item, int oldIndex, int newIndex)
        {
            SpliceCollection(array, oldIndex, 1);
            SpliceCollection(array, newIndex, 0, item);
        }

        public void Dispose()
        {
            _Silenters.Clear();
        }
    }
}
