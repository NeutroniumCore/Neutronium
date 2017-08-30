using Neutronium.Core.JavascriptFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Infra;

namespace Neutronium.Core.Binding.GlueObject
{
    public class BridgeUpdater
    {
        private readonly Action<IJavascriptViewModelUpdater> _UpdateJavascriptObject;
        private List<IJsCsGlue> _ExitingObjects;

        internal IJavascriptSessionCache Cache { get; set; }
        internal bool HasUpdatesOnJavascriptContext => (_UpdateJavascriptObject != null) || (_ExitingObjects?.Count > 0);

        public BridgeUpdater(Action<IJavascriptViewModelUpdater> update)
        {
            _UpdateJavascriptObject = update;
        }

        public BridgeUpdater(IJavascriptSessionCache cache)
        {
            Cache = cache;
        }

        public void UpdateOnJavascriptContext(IJavascriptViewModelUpdater javascriptViewModelUpdater)
        {
            _UpdateJavascriptObject?.Invoke(javascriptViewModelUpdater);
            if (_ExitingObjects == null)
                return;

           _ExitingObjects.ForEach(Cache.RemoveFromJsToCSharp);
            javascriptViewModelUpdater.UnListen(_ExitingObjects.Where(exiting => exiting.Type == JsCsGlueType.Object && exiting.CValue.GetType().HasReadWriteProperties()).Select(glue => glue.JsValue));
        }

        internal void RequestCleanUp(List<IJsCsGlue> toBeCleaned)
        {
            _ExitingObjects = toBeCleaned;
        }
    }
}
