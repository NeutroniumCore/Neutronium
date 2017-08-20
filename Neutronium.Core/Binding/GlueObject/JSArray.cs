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

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JSArray : GlueBase, IJSCSCachableGlue
    {  
        private readonly Type _IndividualType;

        public object CValue { get; }
        public List<IJSCSGlue> Items { get; }     
        public JsCsGlueType Type => JsCsGlueType.Array;
        public virtual IJavascriptObject CachableJSValue => JSValue;

        private uint _JsId;
        public uint JsId => _JsId;
        void IJSCSCachableGlue.SetJsId(uint jsId) => _JsId = jsId;

        public JSArray(List<IJSCSGlue> values, IEnumerable collection, Type individual)
        {
            CValue = collection;
            Items = values;
            _IndividualType = individual; 
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestArrayCreation(Items);
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

        public void UpdateEventArgsFromJavascript(CollectionChanges.CollectionChanges collectionChanges)
        {
            var list = CValue as IList;
            if (list == null) return;

            collectionChanges.IndividualChanges.ForEach(c => ReplayChanges(c, list));
        }

        public BridgeUpdater GetAddUpdater(IJSCSGlue glue, int index)
        {
            Items.Insert(index, glue);
            return new BridgeUpdater( viewModelUpdater => Splice(viewModelUpdater, index, 0, glue));
        }

        public BridgeUpdater GetReplaceUpdater(IJSCSGlue glue, int index)
        {
            Items[index] = glue;
            return new BridgeUpdater( viewModelUpdater => Splice(viewModelUpdater, index, 1, glue));
        }

        public BridgeUpdater GetMoveUpdater(int oldIndex, int newIndex)
        {
            var item = Items[oldIndex];
            Items.RemoveAt(oldIndex);
            Items.Insert(newIndex, item);

            return new BridgeUpdater(viewModelUpdater => MoveJavascriptCollection(viewModelUpdater, item.GetJSSessionValue(), oldIndex, newIndex));
        }

        public BridgeUpdater GetRemoveUpdater(int index)
        {
            Items.RemoveAt(index);
            return new BridgeUpdater(viewModelUpdater => Splice(viewModelUpdater, index, 1));
        }

        public BridgeUpdater GetResetUpdater()
        {
            Items.Clear();
            return new BridgeUpdater(viewModelUpdater => ClearAllJavascriptCollection(viewModelUpdater));
        }

        private void Splice(IJavascriptViewModelUpdater viewModelUpdater, int index, int number, IJSCSGlue glue)
        {
            viewModelUpdater?.SpliceCollection(CachableJSValue, index, number, glue.GetJSSessionValue());
        }

        private void Splice(IJavascriptViewModelUpdater viewModelUpdater, int index, int number)
        {
            viewModelUpdater?.SpliceCollection(CachableJSValue, index, number);
        }

        private void MoveJavascriptCollection(IJavascriptViewModelUpdater viewModelUpdater, IJavascriptObject item, int oldIndex, int newIndex)
        {
            viewModelUpdater?.MoveCollectionItem(CachableJSValue, item, oldIndex, newIndex);
        }

        private void ClearAllJavascriptCollection(IJavascriptViewModelUpdater viewModelUpdater)
        {
            viewModelUpdater?.ClearAllCollection(CachableJSValue);
        }

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.Append("[");
            var count = 0;
            foreach (var it in Items)
            {
                if (count!=0)
                    context.Append(",");

                using (context.PushContext(count++))
                {
                    it.BuilString(context);
                }         
            }
            context.Append("]");
        }

        public override IEnumerable<IJSCSGlue> GetChildren()
        {
            return Items;
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
