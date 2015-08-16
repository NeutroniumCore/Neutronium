using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using MVVM.CEFGlue.Infra;
using MVVM.CEFGlue.Exceptions;
using MVVM.CEFGlue.CefGlueHelper;
using MVVM.CEFGlue.CefSession;
using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;



namespace MVVM.CEFGlue.HTMLBinding
{
    internal class JSArray : GlueBase, IJSObservableBridge
    {
        private IWebView _IWebView;

        public JSArray(IWebView context, IEnumerable<IJSCSGlue> values, IEnumerable collection)
        {
            var dest = values.Select(v => v.JSValue).ToList();
            _IWebView = context;

            var res = _IWebView.Evaluate(() =>
            {
                IJavascriptObject myres = _IWebView.Factory.CreateArray(dest.Count);
                dest.ForEach((el, i) => myres.SetValue(i, el));
                return myres;
            });

            JSValue = res;

            Items = new List<IJSCSGlue>(values);
            CValue = collection; 
            var type = collection.GetElementType();
            IndividualType = _IWebView.Factory.IsTypeBasic(type) ?  type : null;
        }

        private Type IndividualType { get; set; }

        public CollectionChanges GetChanger(IJavascriptObject[] value, IJavascriptObject[] status, IJavascriptObject[] index, IJSCBridgeCache bridge)
        {
            return new CollectionChanges(bridge, value, status, index, IndividualType);
        }

        private void ReplayChanges(IndividualCollectionChange change, IList ilist)
        {
            CefCoreSessionSingleton.Session.UIDispatcher.Run(() =>
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
            IList ilist = CValue as IList;
            if (ilist == null) return;

            iCollectionChanges.IndividualChanges.ForEach(c => ReplayChanges(c, ilist));
        }



        public void Add(IJSCSGlue iIJSCBridge, int Index)
        {
            MappedJSValue.InvokeAsync("silentsplice", _IWebView, _IWebView.Factory.CreateInt(Index), _IWebView.Factory.CreateInt(0), iIJSCBridge.GetJSSessionValue());
            if (Index > Items.Count - 1)
                Items.Add(iIJSCBridge);
            else
                Items.Insert(Index, iIJSCBridge);
        }

        public void Insert(IJSCSGlue iIJSCBridge, int Index)
        {
            MappedJSValue.InvokeAsync("silentsplice", _IWebView, _IWebView.Factory.CreateInt(Index), _IWebView.Factory.CreateInt(1), iIJSCBridge.GetJSSessionValue());
            Items[Index] = iIJSCBridge;
        }

        public void Remove(int Index)
        {
            MappedJSValue.InvokeAsync("silentsplice", _IWebView, _IWebView.Factory.CreateInt(Index), _IWebView.Factory.CreateInt(1));
            Items.RemoveAt(Index);
        }

        public void Reset()
        {
            MappedJSValue.InvokeAsync("silentremoveAll", _IWebView);
            Items.Clear();
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

        public IJavascriptObject JSValue { get; private set; }

        public object CValue { get; private set; }

        public IList<IJSCSGlue> Items { get; private set; }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return Items;
        }

        public JSCSGlueType Type { get { return JSCSGlueType.Array; } }

        public IJavascriptObject MappedJSValue { get; private set; }

        public void SetMappedJSValue(IJavascriptObject ijsobject, IJSCBridgeCache mapper)
        {
            MappedJSValue = ijsobject;
        }

    }
}
