using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neutronium.Core.Binding.CollectionChanges;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using MoreCollection.Extensions;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JSArray : GlueBase, IJSObservableBridge
    {
        private readonly HTMLViewContext _HTMLViewContext;   
        private readonly Type _IndividualType;

        public IJavascriptObject JSValue { get; private set; }
        public object CValue { get; }
        public IList<IJSCSGlue> Items { get; }     
        public JsCsGlueType Type => JsCsGlueType.Array;
        public IJavascriptObject MappedJSValue { get; private set;  }
        private IWebView WebView => _HTMLViewContext.WebView;
        private IJavascriptViewModelUpdater ViewModelUpdater => _HTMLViewContext.ViewModelUpdater;
        private IDispatcher UIDispatcher=> _HTMLViewContext.UIDispatcher;

        public JSArray(HTMLViewContext context, IEnumerable<IJSCSGlue> values, IEnumerable collection)
        {
            _HTMLViewContext = context;
            CValue = collection;
            Items = new List<IJSCSGlue>(values);
            var type = collection.GetElementType();
            _IndividualType = WebView.Factory.IsTypeBasic(type) ?  type : null;
        }
    

        protected override bool LocalComputeJavascriptValue(IJavascriptObjectFactory factory)
        {
            if (JSValue!=null)
                return false;
        
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
            return new Neutronium.Core.Binding.CollectionChanges.CollectionChanges(bridge, changes, _IndividualType);
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
            ViewModelUpdater.SpliceCollection(MappedJSValue, index, number, glue.GetJSSessionValue());
        }

        private void Splice(int index, int number)
        {
            ViewModelUpdater.SpliceCollection(MappedJSValue, index, number);
        }

        private void ClearAllJavascriptCollection()
        {
            ViewModelUpdater.ClearAllCollection(MappedJSValue);
        }

        public void MoveJavascriptCollection(IJavascriptObject item, int oldIndex, int newIndex)
        {
            ViewModelUpdater.MoveCollectionItem(MappedJSValue, item, oldIndex, newIndex);
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

        protected override void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            sb.Append("[");
            bool f = true;
            foreach (var it in Items)
            {
                if (!f)
                    sb.Append(",");
                f = false;
                it.BuilString(sb, alreadyComputed);
            }
            sb.Append("]");
        }

        public override IEnumerable<IJSCSGlue> GetChildren()
        {
            return Items;
        }

        public void SetMappedJSValue(IJavascriptObject ijsobject)
        {
            MappedJSValue = ijsobject;
        }
    }
}
