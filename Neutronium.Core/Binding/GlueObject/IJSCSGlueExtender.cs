using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;
using System;

namespace Neutronium.Core.Binding.GlueObject
{
    public static class IJSCSGlueExtender
    {
        public static bool IsBasic(this IJSCSGlue @this)
        {
            return (@this.Type == JsCsGlueType.Basic);
        }

        public static bool IsBasicNotNull(this IJSCSGlue @this)
        {
            return (@this.CValue != null && @this.Type == JsCsGlueType.Basic);
        }

        private static void GetAllChildren(this IJSCSGlue @this, ISet<IJSCSGlue> res)
        {
            @this.GetChildren().Where(res.Add).ForEach(direct => direct.GetAllChildren(res));
        }

        public static HashSet<IJSCSGlue> GetAllChildren(this IJSCSGlue @this, bool includeMySelf = false)
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
            return (inj != null) ? inj.MappedJSValue : @this.JSValue;
        }

        internal static void AutoMap(this IJSObservableBridge @this)
        {
            @this.SetMappedJSValue(@this.JSValue);
        }

        public static void ApplyOnListenable(this IJSCSGlue @this, IListenableObjectVisitor visitor)
        {
            @this.GetAllChildren(true).ForEach(child => child.ApplyOnSingleListenable(visitor));
        }

        public static void ApplyOnSingleListenable(this IJSCSGlue @this, IListenableObjectVisitor visitor)
        {
            var value = @this.CValue;
            var notifyCollectionChanged = value as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                visitor.OnCollection(notifyCollectionChanged);
                return;
            }

            var notifyPropertyChanged = value as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                visitor.OnObject(notifyPropertyChanged);
                return;
            }            

            if (@this.Type == JsCsGlueType.Command)
                visitor.OnCommand(@this as JSCommand);
        }
    }
}
