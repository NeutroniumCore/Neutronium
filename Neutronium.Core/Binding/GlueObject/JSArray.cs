using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Binding.CollectionChanges;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JSArray : GlueBase, IJSObservableBridge
    {  
        private readonly Type _IndividualType;
        private IJavascriptViewModelUpdater _ViewModelUpdater;

        public IJavascriptObject JSValue { get; private set; }
        public object CValue { get; }
        public IList<IJSCSGlue> Items { get; }     
        public JsCsGlueType Type => JsCsGlueType.Array;
        public IJavascriptObject MappedJSValue { get; private set;  }

        public JSArray(IEnumerable<IJSCSGlue> values, IEnumerable collection, Type individual)
        {
            CValue = collection;
            Items = new List<IJSCSGlue>(values);
            _IndividualType = individual; 
        } 

        protected override bool LocalComputeJavascriptValue(IJavascriptObjectFactory factory, IJavascriptViewModelUpdater updater)
        {
            if (JSValue!=null)
                return false;

            _ViewModelUpdater = updater;
            JSValue = factory.CreateArray(Items.Count);
            return true;
        }

        protected override void AfterChildrenComputeJavascriptValue()
        {
            var dest = Items.Select(v => v.JSValue).ToList();
            dest.ForEach((javascriptObject, index) => JSValue.SetValue(index, javascriptObject));
        }

        public Neutronium.Core.Binding.CollectionChanges.CollectionChanges GetChanger(JavascriptCollectionChanges changes, IJavascriptToCSharpConverter bridge)
        {
            return new CollectionChanges.CollectionChanges(bridge, changes, _IndividualType);
        }

        private void ReplayChanges(IndividualCollectionChange change, IList ilist)
        {
            switch (change.CollectionChangeType)
            {
                case CollectionChangeType.Add:
                if (change.Index == ilist.Count) 
                {
                    ilist.Add(change.Object.CValue);
                    Items.Add(change.Object);
                }
                else 
                {
                    ilist.Insert(change.Index, change.Object.CValue);
                    Items.Insert(change.Index, change.Object);
                }
                break;

                case CollectionChangeType.Remove:
                    ilist.RemoveAt(change.Index);
                    Items.RemoveAt(change.Index);
                break;
            }
        }

        public void UpdateEventArgsFromJavascript(Neutronium.Core.Binding.CollectionChanges.CollectionChanges iCollectionChanges)
        {
            var ilist = CValue as IList;
            if (ilist == null) return;

            iCollectionChanges.IndividualChanges.ForEach(c => ReplayChanges(c, ilist));
        }

        private void Splice(int index, int number, IJSCSGlue glue)
        {
            _ViewModelUpdater?.SpliceCollection(MappedJSValue, index, number, glue.GetJSSessionValue());
        }

        private void Splice(int index, int number)
        {
            _ViewModelUpdater?.SpliceCollection(MappedJSValue, index, number);
        }

        private void ClearAllJavascriptCollection()
        {
            _ViewModelUpdater?.ClearAllCollection(MappedJSValue);
        }

        public void MoveJavascriptCollection(IJavascriptObject item, int oldIndex, int newIndex)
        {
            _ViewModelUpdater?.MoveCollectionItem(MappedJSValue, item, oldIndex, newIndex);
        }

        public void Add(IJSCSGlue jscBridge, int index)
        {
            Splice(index, 0, jscBridge);
            Items.Insert(index, jscBridge);
        }

        public void Replace(IJSCSGlue jscBridge, int index)
        {
            Splice(index, 1, jscBridge);
            Items[index] = jscBridge;
        }

        public void Remove(int index)
        {
            Splice(index, 1) ;
            Items.RemoveAt(index);
        }

        public void Reset()
        {
            ClearAllJavascriptCollection();
            Items.Clear();
        }

        public void Move(int oldIndex, int newIndex)
        {
            var item = Items[oldIndex];
            MoveJavascriptCollection(item.GetJSSessionValue(), oldIndex, newIndex);
            Items.RemoveAt(oldIndex);
            Items.Insert(newIndex, item);
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

        public void SetMappedJSValue(IJavascriptObject jsobject)
        {
            MappedJSValue = jsobject;
        }
    }
}
