using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVM.CEFGlue.Infra;
using System.Collections;

using Xilium.CefGlue;

using MVVM.CEFGlue.Exceptions;
using MVVM.CEFGlue.CefGlueHelper;
using MVVM.CEFGlue.CefSession;



namespace MVVM.CEFGlue.HTMLBinding
{
    internal class JSArray : GlueBase, IJSObservableBridge
    {
        private CefV8Context _CefV8Context;

        public JSArray(CefV8Context context, IEnumerable<IJSCSGlue> values, IEnumerable collection, Type ElementType)
        {
            var dest = values.Select(v => v.JSValue).ToList();
            _CefV8Context = context;

            var res = _CefV8Context.EvaluateAsync(() =>
            {
                _CefV8Context.Enter();
                CefV8Value myres = CefV8Value.CreateArray(dest.Count);
                dest.ForEach((el, i) => myres.SetValue(i, el));
                _CefV8Context.Exit();
                return myres;
            }).Result;

            //var res = CefV8Value.CreateArray(dest.Count);

            JSValue = res;

            Items = new List<IJSCSGlue>(values);
            CValue = collection;
            IndividualType = ElementType;
        }

        private Type IndividualType { get; set; }

        public CollectionChanges GetChanger(CefV8Value[] value, CefV8Value[] status, CefV8Value[] index, IJSCBridgeCache bridge)
        {
            return new CollectionChanges(bridge, value, status, index, IndividualType);
        }

        private void ReplayChanges(IndividualCollectionChange change, IList ilist)
        {
            CefCoreSessionSingleton.Session.Dispatcher.Run(() =>
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
            MappedJSValue.InvokeAsync("silentsplice", _CefV8Context, CefV8Value.CreateInt(Index), CefV8Value.CreateInt(0), iIJSCBridge.GetJSSessionValue());
            if (Index > Items.Count - 1)
                Items.Add(iIJSCBridge);
            else
                Items.Insert(Index, iIJSCBridge);
        }

        public void Insert(IJSCSGlue iIJSCBridge, int Index)
        {
            MappedJSValue.InvokeAsync("silentsplice", _CefV8Context, CefV8Value.CreateInt(Index), CefV8Value.CreateInt(1), iIJSCBridge.GetJSSessionValue());
            Items[Index] = iIJSCBridge;
        }

        public void Remove(int Index)
        {
            MappedJSValue.InvokeAsync("silentsplice", _CefV8Context, CefV8Value.CreateInt(Index), CefV8Value.CreateInt(1));
            Items.RemoveAt(Index);
        }

        public void Reset()
        {
            MappedJSValue.InvokeAsync("silentremoveAll", _CefV8Context);
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

        public CefV8Value JSValue { get; private set; }

        public object CValue { get; private set; }

        public IList<IJSCSGlue> Items { get; private set; }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return Items;
        }

        public JSCSGlueType Type { get { return JSCSGlueType.Array; } }

        public CefV8Value MappedJSValue { get; private set; }

        public void SetMappedJSValue(CefV8Value ijsobject, IJSCBridgeCache mapper)
        {
            MappedJSValue = ijsobject;
        }

    }
}
