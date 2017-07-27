using Neutronium.Core.JavascriptFramework;
using System;

namespace Neutronium.Core.Binding.GlueObject
{
    public class BridgeUpdater
    {
        private Action<IJavascriptViewModelUpdater> _UpdateJavascriptObject = null;
        private Action<IJavascriptViewModelUpdater> _NextUpdateJavascriptObject = null;

        public BridgeUpdater(Action<IJavascriptViewModelUpdater> update)
        {
            _UpdateJavascriptObject = update;
        }

        public BridgeUpdater()
        {
        }

        public void AddAction(Action<IJavascriptViewModelUpdater> update)
        {
            if (_UpdateJavascriptObject == null)
            {
                _UpdateJavascriptObject = update;
                return;
            }
            _NextUpdateJavascriptObject = update;
        }

        public void UpdateJavascriptObject(IJavascriptViewModelUpdater javascriptViewModelUpdater)
        {
            _UpdateJavascriptObject?.Invoke(javascriptViewModelUpdater);
            _NextUpdateJavascriptObject?.Invoke(javascriptViewModelUpdater);
        }
    }
}
