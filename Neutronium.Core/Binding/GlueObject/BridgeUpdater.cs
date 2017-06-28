using Neutronium.Core.JavascriptFramework;
using System;

namespace Neutronium.Core.Binding.GlueObject
{
    public class BridgeUpdater
    {
        private Action<IJavascriptViewModelUpdater> _UpdateJavascriptObject = null;

        public BridgeUpdater(Action<IJavascriptViewModelUpdater> update)
        {
            _UpdateJavascriptObject = update;
        }

        public BridgeUpdater()
        {
        }

        public void UpdateJavascriptObject(IJavascriptViewModelUpdater javascriptViewModelUpdater)
        {
            _UpdateJavascriptObject?.Invoke(javascriptViewModelUpdater);
        }
    }
}
