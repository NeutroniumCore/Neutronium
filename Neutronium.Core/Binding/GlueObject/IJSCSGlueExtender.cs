using System.Collections.Generic;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
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

        public static ISet<IJSCSGlue> GetAllChildren(this IJSCSGlue @this, bool includeMySelf = false)
        {
            var res = new HashSet<IJSCSGlue>();
            if (includeMySelf)
                res.Add(@this);

            @this.GetAllChildren(res);
            return res;
        }

        private static void GetAllChildren(this IJSCSGlue @this, ISet<IJSCSGlue> res)
        {
            var children = @this.GetChildren();
            if (children == null)
                return;

            foreach (var child in children)
            {
                if (res.Add(child))
                {
                    child.GetAllChildren(res);
                }
            }
        }

        public static void VisitAllChildren(this IJSCSGlue @this, Func<IJSCSGlue, bool> visit)
        {
            if (!visit(@this))
                return;

            var res = new HashSet<IJSCSGlue>();
            res.Add(@this);
            @this.VisitAllChildren(visit, res);
        }

        private static void VisitAllChildren(this IJSCSGlue @this, Func<IJSCSGlue, bool> visit, ISet<IJSCSGlue> res)
        {
            var children = @this.GetChildren();
            if (children == null)
                return;

            foreach (var child in children)
            {
                if (res.Add(child) && visit(child))
                {
                    child.VisitAllChildren(visit, res);
                }
            }
        }

        public static ISet<IJSCSGlue> GetAllChildren(this IJSCSGlue @this, Func<JsCsGlueType, bool> filter)
        {
            var res = new HashSet<IJSCSGlue>();
            if (filter(@this.Type))
                return res;
       
            res.Add(@this);
            @this.GetAllChildren(filter, res);
            return res;
        }

        private static void GetAllChildren(this IJSCSGlue @this, Func<JsCsGlueType, bool> filter, ISet<IJSCSGlue> res)
        {
            var children = @this.GetChildren();
            if (children == null)
                return;

            foreach (var child in children)
            {
                if (!filter(child.Type) && res.Add(child))
                {
                    child.GetAllChildren(filter, res);
                }
            }
        }

        public static IJavascriptObject GetJSSessionValue(this IJSCSGlue @this)
        {
            var inj = @this as IJSCSMappedBridge;
            return (inj != null) ? inj.CachableJSValue : @this.JSValue;
        }

        internal static void AutoMap(this IJSCSMappedBridge @this)
        {
            @this.SetMappedJSValue(@this.JSValue);
        }
    }
}
