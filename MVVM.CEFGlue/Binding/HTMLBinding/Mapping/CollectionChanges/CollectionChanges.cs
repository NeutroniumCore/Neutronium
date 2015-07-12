using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.HTMLBinding
{
    public class CollectionChanges :  IComparer<IndividualCollectionChange>
    {
        private IJSCBridgeCache _IJSCBridgeCache;
        private Type _TargetedType;

        public CollectionChanges(IJSCBridgeCache iJSCBridgeCache, CefV8Value[] value, CefV8Value[] status, CefV8Value[] index, Type iTargetedType)
        {
            _IJSCBridgeCache = iJSCBridgeCache;
            _TargetedType = iTargetedType;
            IndividualChanges = GetIndividualChanges(value, status, index).OrderBy(idc => idc, this);
        }


        public IEnumerable<IndividualCollectionChange> IndividualChanges { get; private set; }

        public IEnumerable<IndividualCollectionChange> GetIndividualChanges(CefV8Value[] value, CefV8Value[] status, CefV8Value[] index)
        {
            int Length = value.Length;
            for (int i=0;i<Length;i++)
            {
                yield return new IndividualCollectionChange(
                    status[i].GetStringValue() == "added" ? CollectionChangeType.Add : CollectionChangeType.Remove,
                    index[i].GetIntValue(),
                    _IJSCBridgeCache.GetCachedOrCreateBasic(value[i], _TargetedType));
            }
        }

        public int Compare(IndividualCollectionChange x, IndividualCollectionChange y)
        {
            if (x.CollectionChangeType != y.CollectionChangeType)
                return (x.CollectionChangeType == CollectionChangeType.Add) ? 1 : -1;

            return ((x.CollectionChangeType == CollectionChangeType.Add)? 1 : -1) * (x.Index - y.Index);
        }
    }
}
