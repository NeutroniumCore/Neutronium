using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    public static class IJSCSGlueExtender
    {
        private static void GetAllChildren(this IJSCSGlue @this, bool includeMySelf, ISet<IJSCSGlue> res) 
        {
            if (includeMySelf)
                res.Add(@this);

            foreach (var direct in @this.GetChildren().Where(res.Add)) 
            {
                direct.GetAllChildren(false, res);
            }
        }

        public static bool IsBasic(this IJSCSGlue @this)
        {
            return ((@this.Type == JsCsGlueType.Basic) || ((@this.Type == JsCsGlueType.Basic) && @this.JSValue.IsNull));
        }

        public static IEnumerable<IJSCSGlue> GetAllChildren(this IJSCSGlue @this, bool includeMySelf=false)
        {
            var res = new HashSet<IJSCSGlue>();
            @this.GetAllChildren(includeMySelf,res);
            return res;
        }

        public static IJavascriptObject GetJSSessionValue(this IJSCSGlue @this)
        {
            var inj = @this as IJSObservableBridge;
            return (inj!=null) ?  inj.MappedJSValue : @this.JSValue;    
        }

        internal static void AutoMap(this IJSObservableBridge @this) 
        {
            @this.SetMappedJSValue(@this.JSValue);
        }

        public static void ApplyOnListenable(this IJSCSGlue @this, IListenableObjectVisitor ivisitor)
        {
            foreach (var child in @this.GetAllChildren(true))
            {
                var childvalue = child.CValue;
                var notifyCollectionChanged = childvalue as INotifyCollectionChanged;
                if (notifyCollectionChanged != null)
                {
                    ivisitor.OnCollection(notifyCollectionChanged);
                    continue;
                }

                var notifyPropertyChanged = childvalue as INotifyPropertyChanged;
                if ((notifyPropertyChanged != null) && !(child is IEnumerable))
                    ivisitor.OnObject(notifyPropertyChanged);

                if (child.Type==JsCsGlueType.Command)
                    ivisitor.OnCommand(child as JSCommand);
            }
        }
    }
}
