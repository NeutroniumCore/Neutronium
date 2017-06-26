using System;
using System.Collections.Generic;
using Neutronium.Core;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;

namespace Neutronium.JavascriptFramework.Vue
{
    internal class VueJavascriptViewModelUpdater : IJavascriptViewModelUpdater
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Listener;
        private readonly IDictionary<IJavascriptObject, IJavascriptObject> _Silenters =  new Dictionary<IJavascriptObject, IJavascriptObject>();
        private readonly Lazy<IJavascriptObject> _VueHelper;
        private readonly IWebSessionLogger _Logger;

        public VueJavascriptViewModelUpdater(IWebView webView, IJavascriptObject listener, Lazy<IJavascriptObject> vueHelper, IWebSessionLogger logger)
        {
            _WebView = webView;
            _Listener = listener;
            _VueHelper = vueHelper;
            _Logger = logger;
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
            _WebView.RunAsync(() => 
            {
                array.Invoke("silentSplice", _WebView, _WebView.Factory.CreateInt(oldIndex), _WebView.Factory.CreateInt(1));
                AddUnsafe(array, newIndex, 0, item);
            });
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number)
        {
            array.InvokeAsync("silentSplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number));
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject added) 
        {
            _WebView.RunAsync(() => 
            {
                AddUnsafe(array, index, number, added);
            });
        }

        public void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value, bool isBasic)
        {
            _WebView.RunAsync(() =>
            {
                var silenter = GetOrCreateSilenter(father);
                if (silenter == null)
                {
                    _Logger.Info(() => $"UpdateProperty called during an injection process. Property updated {propertyName}");
                    //may happen if code being call between register and inject
                    //in this case just set attribute value. The value will be register after
                    father.SetValue(propertyName, value);
                    return;
                }
                var forProperty = silenter.GetValue(propertyName);
                forProperty.Invoke("silence", _WebView, value);
                if (!isBasic)
                {
                    InjectUnsafe(value);
                }               
            });
        }

        private void AddUnsafe(IJavascriptObject array, int index, int number, IJavascriptObject value) 
        {
            array.Invoke("silentSplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number), value);
            InjectUnsafe(value);
        }

        private void InjectUnsafe(IJavascriptObject value) 
        {
            _VueHelper.Value.Invoke("inject", _WebView, value, _Listener);
        }

        private IJavascriptObject GetOrCreateSilenter(IJavascriptObject father)
        {
            var candidate = _Silenters.GetOrAddEntity(father, _ => father.GetValue("__silenter"));
            if (candidate.IsUndefined)
            {
                _Silenters.Remove(father);
                return null;
            }
            return candidate;
        }
    }
}
