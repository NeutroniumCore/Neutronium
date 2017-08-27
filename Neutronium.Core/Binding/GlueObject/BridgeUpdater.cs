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
        private readonly List<IJsCsGlue> _ExitingObjects = new List<IJsCsGlue>();

        internal IJavascriptSessionCache Cache { get; set; }
        internal bool HasUpdatesOnJavascriptContext => (_UpdateJavascriptObject != null) || (_EntityToUnlisten.Count != 0);

        public BridgeUpdater(Action<IJavascriptViewModelUpdater> update)
        {
            _UpdateJavascriptObject = update;
        }

        public BridgeUpdater()
        {
        }

        public void RequestUnlisten(IJavascriptObject exiting)
        {
            _EntityToUnlisten.Add(exiting);
        }

        public void RequestJsCacheRemove(IJsCsGlue exiting)
        {
            _ExitingObjects.Add(exiting);
        }

        public void UpdateOnJavascriptContext(IJavascriptViewModelUpdater javascriptViewModelUpdater)
        {
            _UpdateJavascriptObject?.Invoke(javascriptViewModelUpdater);

            if (Cache != null)
                _ExitingObjects.ForEach(Cache.RemoveFromJsToCSharp);

            if (_EntityToUnlisten.Count == 0)
                return;

            javascriptViewModelUpdater.UnListen(_EntityToUnlisten);
        }
    }
}
