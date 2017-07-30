using System;
using Neutronium.Core;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;
using MoreCollection.Extensions;
using System.Linq;
using Neutronium.Core.Extension;

namespace Neutronium.JavascriptFramework.Vue
{
    internal class VueJavascriptViewModelUpdater : IJavascriptViewModelUpdater
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Listener;
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

        public void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value, bool childAllowWrite)
        {
            var functionName = childAllowWrite ? "silentChangeAndInject" : "silentChange";
            _VueHelper.Value.InvokeNoResult(functionName, _WebView, father, _WebView.Factory.CreateString(propertyName), value, _Listener);
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

        public void UnListen(IList<IJavascriptObject> elementsToUnlisten)
        {
            var helper = _VueHelper.Value;
            _WebView.Slice(elementsToUnlisten).ForEach(elements => helper.InvokeNoResult("disposeSilenters", _WebView, elements));
        }
    }
}
