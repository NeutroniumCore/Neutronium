using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;

namespace Neutronium.Core.Binding.GlueObject
{
    public static class IJSCSGlueExtender
    {
        private static void GetAllChildren(this IJSCSGlue @this, ISet<IJSCSGlue> res) 
        {
            @this.GetChildren().Where(res.Add).ForEach(direct => direct.GetAllChildren(res));
        }

        public static bool IsBasic(this IJSCSGlue @this)
        {
            return (@this.Type == JsCsGlueType.Basic);
        }

        public static IEnumerable<IJSCSGlue> GetAllChildren(this IJSCSGlue @this, bool includeMySelf=false)
        {
            var res = new HashSet<IJSCSGlue>();
            if (includeMySelf)
                res.Add(@this);

            @this.GetAllChildren(res);
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
