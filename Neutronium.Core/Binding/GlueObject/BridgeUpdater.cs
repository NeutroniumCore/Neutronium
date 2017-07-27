using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.GlueObject
{
    public class BridgeUpdater
    {
        private readonly Action<IJavascriptViewModelUpdater> _UpdateJavascriptObject = null;
        private readonly List<IJavascriptObject> _EntityToUnlisten = new List<IJavascriptObject>();

        public BridgeUpdater(Action<IJavascriptViewModelUpdater> update)
        {
            _UpdateJavascriptObject = update;
        }

        public BridgeUpdater()
        {
        }

        public void AddToUnlisten(IJavascriptObject exiting)
        {
            _EntityToUnlisten.Add(exiting);
        }

        public void UpdateJavascriptObject(IJavascriptViewModelUpdater javascriptViewModelUpdater)
        {
            _UpdateJavascriptObject?.Invoke(javascriptViewModelUpdater);

            if (_EntityToUnlisten.Count == 0)
                return;

            javascriptViewModelUpdater.UnListen(_EntityToUnlisten));
        }
    }
}
