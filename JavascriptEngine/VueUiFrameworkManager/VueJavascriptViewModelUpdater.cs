using System;
using MVVM.HTML.Core.JavascriptUIFramework;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using System.Collections.Generic;
using MVVM.HTML.Core.Infra;

namespace VueUiFramework
{
    internal class VueJavascriptViewModelUpdater : IJavascriptViewModelUpdater
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Listener;
        private readonly IDictionary<IJavascriptObject, IJavascriptObject> _Silenters =  new Dictionary<IJavascriptObject, IJavascriptObject>();
        private readonly Lazy<IJavascriptObject> _VueHelper;

        public VueJavascriptViewModelUpdater(IWebView webView, IJavascriptObject listener, Lazy<IJavascriptObject> vueHelper)
        {
            _WebView = webView;
            _Listener = listener;
            _VueHelper = vueHelper;
        }

        public void ClearAllCollection(IJavascriptObject array)
        {
            _WebView.RunAsync(() =>
            {
                var length = array.GetArrayLength();
                array.Invoke("silentSplice", _WebView, _WebView.Factory.CreateInt(0), _WebView.Factory.CreateInt(length));
            });
        }

        public void Dispose()
        {
            _Silenters.Clear();
        }

        public void MoveCollectionItem(IJavascriptObject array, IJavascriptObject item, int oldIndex, int newIndex)
        {
            SpliceCollection(array, oldIndex, 1);
            SpliceCollection(array, newIndex, 0, item);
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number)
        {
            array.InvokeAsync("silentSplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number));
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject added)
        {
            array.InvokeAsync("silentSplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number), added);
        }

        public void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value)
        {
            _WebView.RunAsync(() =>
            {
                var silenter = GetOrCreateSilenter(father);
                var forProperty = silenter.GetValue(propertyName);
                forProperty.Invoke("silence", _WebView, value);
                _VueHelper.Value.Invoke("inject", _WebView, value, _Listener);
            });
        }

        private IJavascriptObject GetOrCreateSilenter(IJavascriptObject father)
        {
            return _Silenters.FindOrCreateEntity(father, _ => father.GetValue("__silenter"));
        }
    }
}
