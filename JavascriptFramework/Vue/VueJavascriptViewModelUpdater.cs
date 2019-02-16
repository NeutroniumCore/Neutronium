using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;
using MoreCollection.Extensions;
using Neutronium.Core.Extension;

namespace Neutronium.JavascriptFramework.Vue
{
    internal class VueJavascriptViewModelUpdater : IJavascriptViewModelUpdater
    {
        private readonly IWebView _WebView;
        private readonly Lazy<IJavascriptObject> _VueHelper;
        private readonly IJavascriptObject[] _ArgumentsforUpdate = new IJavascriptObject[3];
        private VueVmUpdater _Updater;
        private VueVmUpdater Updater => _Updater ?? (_Updater = new VueVmUpdater(_VueHelper.Value));

        private readonly Dictionary<string, IJavascriptObject> _Properties = new Dictionary<string, IJavascriptObject>();

        public VueJavascriptViewModelUpdater(IWebView webView, Lazy<IJavascriptObject> vueHelper)
        {
            _WebView = webView;
            _VueHelper = vueHelper;
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

        public void AddProperty(IJavascriptObject father, string propertyName, IJavascriptObject value)
        {
            var updater = Updater;
            var property = _Properties.GetOrAddEntity(propertyName, CreateProperty);
            updater.AddProperty.ExecuteFunctionNoResult(_WebView, null, GetArgumentsforUpdate(father, property, value));
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject added)
        {
            Add(array, index, number, added);
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IList<IJavascriptObject> items)
        {
            var parameters = new IJavascriptObject[items.Count + 2];
            parameters[0] = _WebView.Factory.CreateInt(index);
            parameters[1] = _WebView.Factory.CreateInt(number);
            var idx = 2;
            items.ForEach(item => parameters[idx++] = item);

            array.InvokeNoResult("silentSplice", _WebView, parameters);
            items.ForEach(Inject);
        }

        public void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value, bool childAllowWrite)
        {
            var updater = Updater;
            var function = childAllowWrite ? updater.ChangeAndInject : updater.Change;
            var property = _Properties.GetOrAddEntity(propertyName, CreateProperty);
            function.ExecuteFunctionNoResult(_WebView, null, GetArgumentsforUpdate(father, property, value));
        }

        private IJavascriptObject[] GetArgumentsforUpdate(IJavascriptObject father, IJavascriptObject property, IJavascriptObject value)
        {
            _ArgumentsforUpdate[0] = father;
            _ArgumentsforUpdate[1] = property;
            _ArgumentsforUpdate[2] = value;
            return _ArgumentsforUpdate;
        }

        private IJavascriptObject CreateProperty(string propertyName) => _WebView.Factory.CreateString(propertyName);

        private void Add(IJavascriptObject array, int index, int number, IJavascriptObject value)
        {
            array.InvokeNoResult("silentSplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number), value);
            Inject(value);
        }

        private void Inject(IJavascriptObject value)
        {
            _VueHelper.Value.InvokeNoResult("inject", _WebView, value);
        }

        public void UnListen(IEnumerable<IJavascriptObject> elementsToUnlisten)
        {
            var helper = _VueHelper.Value;
            _WebView.Slice(elementsToUnlisten).ForEach(elements => helper.InvokeNoResult("disposeSilenters", _WebView, elements));
        }

        public void InjectDetached(IJavascriptObject javascriptObject)
        {
            Updater.InjectDetached.ExecuteFunctionNoResult(_WebView, null, javascriptObject);
        }
    }
}
