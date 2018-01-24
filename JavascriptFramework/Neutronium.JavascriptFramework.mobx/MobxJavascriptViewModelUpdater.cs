using System;
using System.Collections.Generic;
using MoreCollection.Extensions;
using Neutronium.Core;
using Neutronium.Core.Extension;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.JavascriptFramework.mobx 
{
    public class MobxJavascriptViewModelUpdater : IJavascriptViewModelUpdater 
    {
        private readonly IWebView _WebView;
        private readonly IJavascriptObject _Listener;
        private readonly IWebSessionLogger _Logger;
        private readonly Lazy<IJavascriptObject> _MobxHelperLazy;
        private MobxVmUpdater _Updater;
        private readonly Dictionary<string, IJavascriptObject> _Properties = new Dictionary<string, IJavascriptObject>();
        private readonly IJavascriptObject[] _ArgumentsforUpdate = new IJavascriptObject[3];

        private MobxVmUpdater Updater => _Updater ?? (_Updater = new MobxVmUpdater(_MobxHelperLazy.Value));

        public MobxJavascriptViewModelUpdater(IWebView webView, Lazy<IJavascriptObject> helper, IJavascriptObject listener, IWebSessionLogger logger) 
        {
            _WebView = webView;
            _Listener = listener;
            _Logger = logger;
            _MobxHelperLazy = helper;
        }

        public void AddProperty(IJavascriptObject father, string propertyName, IJavascriptObject value) 
        {
            var property = _Properties.GetOrAddEntity(propertyName, CreateProperty);
            Updater.AddProperty.ExecuteFunctionNoResult(_WebView, null, father, property, value);
        }

        public void ClearAllCollection(IJavascriptObject array)
        {
            Updater.ClearCollection.ExecuteFunctionNoResult(_WebView, null, array);
        }

        public void InjectDetached(IJavascriptObject javascriptObject) 
        {
            UpdateVm(javascriptObject);
        }

        public void UnListen(IEnumerable<IJavascriptObject> elementsToUnlisten)
        {
            var unlisten = Updater.UnListen;
            _WebView.Slice(elementsToUnlisten).ForEach(elements => unlisten.ExecuteFunctionNoResult(_WebView, null, elements));
        }

        public void MoveCollectionItem(IJavascriptObject array, IJavascriptObject item, int oldIndex, int newIndex)
        {
            SilentSplice(array, oldIndex, 1);
            SilentSplice(array, newIndex, 0, item);
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject item)
        {
            SilentSplice(array, index, number, item);
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number)
        {
            SilentSplice(array, index, number);
        }

        private void SilentSplice(IJavascriptObject array, int firstArgument, int secondArgument, IJavascriptObject item)
        {
            Updater.SilentSplice.ExecuteFunctionNoResult(_WebView, null, array, _WebView.Factory.CreateInt(firstArgument), _WebView.Factory.CreateInt(secondArgument), item);
            UpdateVm(item);
        }

        private void SilentSplice(IJavascriptObject array, int firstArgument, int secondArgument)
        {
            Updater.SilentSplice.ExecuteFunctionNoResult(_WebView, null, array, _WebView.Factory.CreateInt(firstArgument), _WebView.Factory.CreateInt(secondArgument));
        }

        private void UpdateVm(IJavascriptObject value)
        {
            Updater.UpdateVm.ExecuteFunctionNoResult(_WebView, null, value);
        }

        public void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value, bool childAllowWrite)
        {
            var updater = Updater;
            var property = _Properties.GetOrAddEntity(propertyName, CreateProperty);
            var changer = childAllowWrite ? updater.SilentChangeUpdate : updater.SilentChange;
            changer.ExecuteFunctionNoResult(_WebView, null, GetArgumentsforUpdate(father, property, value));
        }

        private IJavascriptObject CreateProperty(string propertyName) => _WebView.Factory.CreateString(propertyName);

        private IJavascriptObject[] GetArgumentsforUpdate(IJavascriptObject father, IJavascriptObject property, IJavascriptObject value)
        {
            _ArgumentsforUpdate[0] = father;
            _ArgumentsforUpdate[1] = property;
            _ArgumentsforUpdate[2] = value;
            return _ArgumentsforUpdate;
        }
    }
}
