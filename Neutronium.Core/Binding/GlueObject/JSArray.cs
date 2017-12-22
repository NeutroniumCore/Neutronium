using System;
using System.Collections;
using System.Collections.Generic;
using Neutronium.Core.Binding.CollectionChanges;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;
using System.Collections.Specialized;
using System.Linq;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JsArray : GlueBase, IJsCsCachableGlue
    {
        private readonly Type _IndividualType;

        public object CValue { get; }
        public List<IJsCsGlue> Items { get; private set; }
        public JsCsGlueType Type => JsCsGlueType.Array;
        public virtual IJavascriptObject CachableJsValue => JsValue;
        public uint JsId { get; private set; }

        void IJsCsCachableGlue.SetJsId(uint jsId) => JsId = jsId;

        public JsArray(IEnumerable collection, Type individual)
        {
            CValue = collection;            
            _IndividualType = individual;
        }

        internal void SetChildren(List<IJsCsGlue> values)
        {
            Items = values;
        }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            if (!visit(this))
                return;

            foreach (var item in Items)
            {
                item.VisitDescendants(visit);
            }
        }

        public void VisitChildren(Action<IJsCsGlue> visit) 
        {
            foreach (var item in Items)
                visit(item);
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestArrayCreation(Items);
        }

        public virtual void SetJsValue(IJavascriptObject value, IJavascriptSessionCache sessionCache)
        {
            SetJsValue(value);
            sessionCache.Cache(this);
        }

        public CollectionChanges.CollectionChanges GetChanger(JavascriptCollectionChanges changes, IJavascriptToCSharpConverter bridge)
        {
            return new CollectionChanges.CollectionChanges(bridge, changes, _IndividualType);
        }

        private void ReplayChanges(IndividualCollectionChange change, IList list)
        {
            switch (change.CollectionChangeType)
            {
                case CollectionChangeType.Add:
                    if (change.Index == list.Count)
                    {
                        list.Add(change.Object.CValue);
                        Items.Add(change.Object);
                    }
                    else
                    {
                        list.Insert(change.Index, change.Object.CValue);
                        Items.Insert(change.Index, change.Object);
                    }
                    break;

                case CollectionChangeType.Remove:
                    list.RemoveAt(change.Index);
                    Items.RemoveAt(change.Index);
                    break;
            }
        }

        public void UpdateEventArgsFromJavascript(CollectionChanges.CollectionChanges collectionChanges, BridgeUpdater updater)
        {
            var list = CValue as IList;
            if (list == null) return;

            collectionChanges.IndividualChanges.ForEach(c => ReplayChanges(c, list));

            collectionChanges.IndividualChanges.Where(ch => ch.CollectionChangeType == CollectionChangeType.Add)
                .ForEach(ch => ch.Object.AddRef());

            collectionChanges.IndividualChanges.Where(ch => ch.CollectionChangeType == CollectionChangeType.Remove)
                .ForEach(ch => CheckForRemove(updater, ch.Object));
        }

        public BridgeUpdater GetAddUpdater(IJsCsGlue glue, int index)
        {
            Items.Insert(index, glue);
            glue.AddRef();
            return new BridgeUpdater(viewModelUpdater => Splice(viewModelUpdater, index, 0, glue));
        }

        private static BridgeUpdater CheckForRemove(BridgeUpdater updater, IJsCsGlue glue)
        {
            if (glue.Release())
                updater.Remove(glue);
            return updater;
        }

        public BridgeUpdater GetReplaceUpdater(IJsCsGlue glue, int index)
        {
            var bridgeUpdater = new BridgeUpdater(viewModelUpdater => Splice(viewModelUpdater, index, 1, glue));
            var old = Items[index];
            Items[index] = glue.AddRef();
            return CheckForRemove(bridgeUpdater, old);
        }

        public BridgeUpdater GetMoveUpdater(int oldIndex, int newIndex)
        {
            var item = Items[oldIndex];
            Items.RemoveAt(oldIndex);
            Items.Insert(newIndex, item);
            return new BridgeUpdater(viewModelUpdater => MoveJavascriptCollection(viewModelUpdater, item.GetJsSessionValue(), oldIndex, newIndex));
        }

        public BridgeUpdater GetRemoveUpdater(int index)
        {
            var bridgeUpdater = new BridgeUpdater(viewModelUpdater => Splice(viewModelUpdater, index, 1));
            var old = Items[index];
            Items.RemoveAt(index);
            return CheckForRemove(bridgeUpdater, old);
        }

        public BridgeUpdater GetResetUpdater()
        {
            var bridgeUpdater = new BridgeUpdater(ClearAllJavascriptCollection);
            foreach (var item in Items)
            {
                CheckForRemove(bridgeUpdater, item);
            }
            Items.Clear();
            return bridgeUpdater;
        }

        private void Splice(IJavascriptViewModelUpdater viewModelUpdater, int index, int number, IJsCsGlue glue)
        {
            viewModelUpdater?.SpliceCollection(CachableJsValue, index, number, glue.GetJsSessionValue());
        }

        private void Splice(IJavascriptViewModelUpdater viewModelUpdater, int index, int number)
        {
            viewModelUpdater?.SpliceCollection(CachableJsValue, index, number);
        }

        private void MoveJavascriptCollection(IJavascriptViewModelUpdater viewModelUpdater, IJavascriptObject item, int oldIndex, int newIndex)
        {
            viewModelUpdater?.MoveCollectionItem(CachableJsValue, item, oldIndex, newIndex);
        }

        private void ClearAllJavascriptCollection(IJavascriptViewModelUpdater viewModelUpdater)
        {
            viewModelUpdater?.ClearAllCollection(CachableJsValue);
        }

        protected override void ComputeString(IDescriptionBuilder context)
        {
            context.Append("[");
            var count = 0;
            foreach (var it in Items)
            {
                if (count != 0)
                    context.Append(",");

                using (context.PushContext(count++))
                {
                    it.BuilString(context);
                }
            }
            context.Append("]");
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
            var notifyCollectionChanged = CValue as INotifyCollectionChanged;
            if (notifyCollectionChanged == null)
                return;

            listener.OnCollection(notifyCollectionChanged);
        }
    }
}
