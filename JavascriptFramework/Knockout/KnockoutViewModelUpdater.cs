using MoreCollection.Extensions;
using Neutronium.Core;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;

namespace Neutronium.JavascriptFramework.Knockout
{
    internal class KnockoutViewModelUpdater : IJavascriptViewModelUpdater, IDisposable
    {
        private readonly IWebView _WebView;
        private readonly IWebSessionLogger _Logger;

        private readonly IDictionary<IJavascriptObject, IDictionary<string, IJavascriptObject>> _Silenters =
                    new Dictionary<IJavascriptObject, IDictionary<string, IJavascriptObject>>();

        internal KnockoutViewModelUpdater(IWebView webView, IWebSessionLogger logger)
        {
            _WebView = webView;
            _Logger = logger;
        }

        public void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value, bool childAllowWrite)
        {
            var silenter = GetSilenter(father, propertyName);
            if (silenter != null)
            {
                Silent(silenter, value);
                return;
            }

            silenter = GetOrCreateSilenter(father, propertyName);
            Silent(silenter, value);
        }

        public void AddProperty(IJavascriptObject father, string propertyName, IJavascriptObject value)
        {
            _Logger.Error("adding property not supported by knockout pluggin");
        }

        private IJavascriptObject GetSilenter(IJavascriptObject father, string propertyName)
        {
            return _Silenters.GetOrDefault(father)?.GetOrDefault(propertyName);
        }

        private IJavascriptObject GetOrCreateSilenter(IJavascriptObject father, string propertyName)
        {
            var dic = _Silenters.GetOrAddEntity(father, _ => new Dictionary<string, IJavascriptObject>());
            return dic.GetOrAddEntity(propertyName, father.GetValue);
        }

        private void Silent(IJavascriptObject silenter, IJavascriptObject value)
        {
            silenter.Invoke("silent", _WebView, value);
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject added)
        {
            array.InvokeAsync("silentsplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number), added);
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject[] items)
        {
            var parameters = new IJavascriptObject[items.Length + 2];
            parameters[0] = _WebView.Factory.CreateInt(index);
            parameters[1] = _WebView.Factory.CreateInt(number);
            var idx = 2;
            items.ForEach(item => parameters[idx++] = item);
            array.InvokeAsync("silentsplice", _WebView, parameters);
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

        public void UnListen(IEnumerable<IJavascriptObject> inClassName)
        {
        }

        public void InjectDetached(IJavascriptObject javascriptObject)
        {
        }
    }
}
