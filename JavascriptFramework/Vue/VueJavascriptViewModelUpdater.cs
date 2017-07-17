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
        private readonly IDictionary<IJavascriptObject, IJavascriptObject> _Silenters = new Dictionary<IJavascriptObject, IJavascriptObject>();
        private readonly Lazy<IJavascriptObject> _VueHelper;
        private readonly IWebSessionLogger _Logger;

        public VueJavascriptViewModelUpdater(IWebView webView, IJavascriptObject listener, Lazy<IJavascriptObject> vueHelper, IWebSessionLogger logger)
        {
            _WebView = webView;
            _Listener = listener;
            _VueHelper = vueHelper;
            _Logger = logger;
        }

        public void Dispose()
        {
            _Silenters.Clear();
        }

        public void ClearAllCollection(IJavascriptObject array)
        {
            var length = array.GetArrayLength();
            array.InvokeNoResult("silentSplice", _WebView, _WebView.Factory.CreateInt(0), _WebView.Factory.CreateInt(length));
        }

        public void MoveCollectionItem(IJavascriptObject array, IJavascriptObject item, int oldIndex, int newIndex)
        {   
            array.InvokeNoResult("silentSplice", _WebView, _WebView.Factory.CreateInt(oldIndex), _WebView.Factory.CreateInt(1));
            Add(array, newIndex, 0, item);
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number)
        {
            array.InvokeNoResult("silentSplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number));
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject added)
        {
            Add(array, index, number, added);
        }

        public void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value, UpdateContext context)
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
            silenter.InvokeNoResult("silentChange", _WebView, _WebView.Factory.CreateString(propertyName), value);
            if (!context.ChildAllowWrite)
                return;

            Inject(value);
        }

        private void Add(IJavascriptObject array, int index, int number, IJavascriptObject value)
        {
            array.InvokeNoResult("silentSplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number), value);
            Inject(value);
        }

        private void Inject(IJavascriptObject value)
        {
            _VueHelper.Value.InvokeNoResult("inject", _WebView, value, _Listener);
        }

        private IJavascriptObject GetOrCreateSilenter(IJavascriptObject father)
        {
            var res = _Silenters.GetOrAddEntity(father, _ =>
            {
                var candidate = father.GetValue("__silenter");
                return (candidate.IsUndefined) ? null : candidate;
            });
            if (res == null)
            {
                _Silenters.Remove(father);
            }
            return res;
        }
    }
}
