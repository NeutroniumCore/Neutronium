using Neutronium.Core.JavascriptFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    public class BridgeUpdater
    {
        private readonly Action<IJavascriptViewModelUpdater> _UpdateJavascriptObject;
        private HashSet<IJsCsGlue> _ExitingObjects;

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
            javascriptViewModelUpdater.UnListen(_ExitingObjects.Where(exiting => (exiting as JsGenericObject)?.HasReadWriteProperties == true).Select(glue => glue.JsValue));
        }

        internal void CleanAfterChangesOnUiThread(ObjectChangesListener offListener)
        {
            if (_ExitingObjects==null)
                return;

            foreach (var exiting in _ExitingObjects)
            {
                exiting.ApplyOnListenable(offListener);
                Cache.RemoveFromCSharpToJs(exiting);
            }
        }

        internal BridgeUpdater Remove(IJsCsGlue old)
        {
            if (old == null)
                return this;

            _ExitingObjects = _ExitingObjects ?? new HashSet<IJsCsGlue>();
            CollectAllRemoved(old, _ExitingObjects);
            return this;
        }

        private static void CollectAllRemoved(IJsCsGlue old, ISet<IJsCsGlue> toRemove)
        {
            if (!toRemove.Add(old))
                return;

            old.Children?.ForEach(child =>
            {
                if (child.Release())
                    CollectAllRemoved(child, toRemove);
            });
        }
    }
}
