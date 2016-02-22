using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Window;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core.HTMLBinding
{
    internal class JSArray : GlueBase, IJSObservableBridge
    {
        private readonly HTMLViewContext _HTMLViewContext;   
        private readonly Type _IndividualType;

        public IJavascriptObject JSValue { get; private set; }
        public object CValue { get; private set; }
        public IList<IJSCSGlue> Items { get; private set; }     
        public JSCSGlueType Type { get { return JSCSGlueType.Array; } }
        public IJavascriptObject MappedJSValue { get; private set; }
        private IWebView WebView { get { return _HTMLViewContext.WebView; } }
        private IJavascriptSessionInjector Injector { get { return _HTMLViewContext.JavascriptSessionInjector; } }
        private IDispatcher UIDispatcher { get { return _HTMLViewContext.UIDispatcher; } }

        public JSArray(HTMLViewContext context, IEnumerable<IJSCSGlue> values, IEnumerable collection)
        {
            _HTMLViewContext = context;
            CValue = collection;
            Items = new List<IJSCSGlue>(values);

            var dest = values.Select(v => v.JSValue).ToList();    
            JSValue = WebView.Evaluate(() => WebView.Factory.CreateArray(dest));
            var type = collection.GetElementType();
            _IndividualType = WebView.Factory.IsTypeBasic(type) ?  type : null;
        }

        public CollectionChanges GetChanger(JavascriptCollectionChanges changes, IJavascriptToCSharpConverter bridge)
        {
            return new CollectionChanges(bridge, changes, _IndividualType);
        }

        private void ReplayChanges(IndividualCollectionChange change, IList ilist)
        {
            UIDispatcher.Run(() => 
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
            });
        }

        public void UpdateEventArgsFromJavascript(CollectionChanges iCollectionChanges)
        {
            var ilist = CValue as IList;
            if (ilist == null) return;

            iCollectionChanges.IndividualChanges.ForEach(c => ReplayChanges(c, ilist));
        }

        private void Splice(int index, int number, IJSCSGlue glue)
        {
            Injector.SpliceCollection(MappedJSValue, index, number, glue.GetJSSessionValue());
        }

        private void Splice(int index, int number)
        {
            Injector.SpliceCollection(MappedJSValue, index, number);
        }

        private void ClearAllJavascriptCollection()
        {
            Injector.ClearAllCollection(MappedJSValue);
        }

        public void MoveJavascriptCollection(IJavascriptObject item, int oldIndex, int newIndex)
        { 
            Injector.MoveCollectionItem(MappedJSValue, item, oldIndex, newIndex);
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

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return Items;
        }

        public void SetMappedJSValue(IJavascriptObject ijsobject)
        {
            MappedJSValue = ijsobject;
        }
    }
}
