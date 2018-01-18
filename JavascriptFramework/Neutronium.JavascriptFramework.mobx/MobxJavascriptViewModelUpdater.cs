using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }
    }
}
