using System;
using System.Collections.Generic;
using MoreCollection.Extensions;
using Neutronium.Core;
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
            throw new NotImplementedException();
        }

        public void ClearAllCollection(IJavascriptObject array)
        {
            throw new NotImplementedException();
        }

        public void InjectDetached(IJavascriptObject javascriptObject) 
        {
            throw new NotImplementedException();
        }

        public void MoveCollectionItem(IJavascriptObject array, IJavascriptObject item, int oldIndex, int newIndex)
        {
            throw new NotImplementedException();
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number, IJavascriptObject item)
        {
            throw new NotImplementedException();
        }

        public void SpliceCollection(IJavascriptObject array, int index, int number)
        {
            throw new NotImplementedException();
        }

        public void UnListen(IEnumerable<IJavascriptObject> elementsToUnlisten) 
        {
            throw new NotImplementedException();
        }

        public void UpdateProperty(IJavascriptObject father, string propertyName, IJavascriptObject value, bool childAllowWrite)
        {
            var updater = Updater;
            var property = _Properties.GetOrAddEntity(propertyName, CreateProperty);
            updater.Change.ExecuteFunctionNoResult(_WebView, null, GetArgumentsforUpdate(father, property, value));

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
