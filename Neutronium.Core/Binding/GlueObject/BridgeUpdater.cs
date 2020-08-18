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

        internal bool HasUpdatesOnJavascriptContext => (_UpdateJavascriptObject != null) || (_ExitingObjects?.Count > 0);

        public BridgeUpdater(Action<IJavascriptViewModelUpdater> update = null)
        {
            _UpdateJavascriptObject = update;
        }

        public void UpdateOnJavascriptContext(IJavascriptViewModelUpdater javascriptViewModelUpdater, IJavascriptSessionCache cache)
        {
            _UpdateJavascriptObject?.Invoke(javascriptViewModelUpdater);
            if (_ExitingObjects == null)
                return;

            _ExitingObjects.ForEach(cache.RemoveFromJsToCSharp);
            javascriptViewModelUpdater.UnListen(_ExitingObjects.Where(exiting => (exiting as JsGenericObject)?.HasReadWriteProperties == true).Select(glue => glue.JsValue));
        }

        internal void CleanAfterChangesOnUiThread(ObjectChangesListener offListener, IJavascriptSessionCache cache)
        {
            if (_ExitingObjects==null)
                return;

            foreach (var exiting in _ExitingObjects)
            {
                exiting.ApplyOnListenable(offListener);
                cache.RemoveFromCSharpToJs(exiting);
            }
        }

        internal BridgeUpdater CheckForRemove(IEnumerable<IJsCsGlue> glues)
        {
            glues.ForEach(CheckForRemoveNoReturn);
            return this;
        }

        internal BridgeUpdater CheckForRemove(IJsCsGlue old)
        {
            CheckForRemoveNoReturn(old);
            return this;
        }

        internal void CheckForRemoveNoReturn(IJsCsGlue old)
        {
            if (old?.Release() != true)
                return;

            _ExitingObjects = _ExitingObjects ?? new HashSet<IJsCsGlue>();
            CollectAllRemoved(old, _ExitingObjects);
        }

        private static void CollectAllRemoved(IJsCsGlue old, ISet<IJsCsGlue> toRemove)
        {
            if (!toRemove.Add(old))
                return;

            old.VisitChildren(child =>
            {
                if (child.Release())
                    CollectAllRemoved(child, toRemove);
            });
        }
    }
}
