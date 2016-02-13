using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Window;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    internal class JSArray : GlueBase, IJSObservableBridge
    {
        private readonly IWebView _WebView;
        private readonly IDispatcher _UIDispatcher;
        private readonly Type _IndividualType;

        public IJavascriptObject JSValue { get; private set; }
        public object CValue { get; private set; }
        public IList<IJSCSGlue> Items { get; private set; }
      
        public JSCSGlueType Type { get { return JSCSGlueType.Array; } }
        public IJavascriptObject MappedJSValue { get; private set; }

        public JSArray(IWebView webView, IDispatcher uiDispatcher, IEnumerable<IJSCSGlue> values, IEnumerable collection)
        {
            _UIDispatcher = uiDispatcher;
            _WebView = webView;
            CValue = collection;
            Items = new List<IJSCSGlue>(values);

            var dest = values.Select(v => v.JSValue).ToList();    
            JSValue = _WebView.Evaluate(() => _WebView.Factory.CreateArray(dest));
            var type = collection.GetElementType();
            _IndividualType = _WebView.Factory.IsTypeBasic(type) ?  type : null;
        }

        public CollectionChanges GetChanger(JavascriptCollectionChanges changes, IJavascriptToCSharpConverter bridge)
        {
            return new CollectionChanges(bridge, changes, _IndividualType);
        }

        private void ReplayChanges(IndividualCollectionChange change, IList ilist)
        {
            _UIDispatcher.Run(() => 
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

#region Knockout
        private void Splice(int index, int number, IJSCSGlue glue)
        {
            MappedJSValue.InvokeAsync("silentsplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number), glue.GetJSSessionValue());
        }

        private void Splice(int index, int number)
        {
            MappedJSValue.InvokeAsync("silentsplice", _WebView, _WebView.Factory.CreateInt(index), _WebView.Factory.CreateInt(number));
        }

        private void ClearAllJavascriptCollection()
        {
            MappedJSValue.InvokeAsync("silentremoveAll", _WebView);
        }
#endregion

        public void MoveJavascriptCollection(int oldIndex, int newIndex)
        {
            var item = Items[oldIndex];
            Splice(oldIndex, 1);
            Splice(newIndex, 0, item);
        }

        private void InsertAt(int index, IJSCSGlue jscBridge)
        {
            //if (index > Items.Count - 1)
            //    Items.Add(jscBridge);
            //else
                Items.Insert(index, jscBridge);
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
            MoveJavascriptCollection(oldIndex, newIndex);
            var item = Items[oldIndex];
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
