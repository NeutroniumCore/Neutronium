using System.Collections.Generic;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;

namespace Neutronium.Core.Binding.GlueObject
{
    public static class JsCsGlueExtender
    {
        public static bool IsBasic(this IJsCsGlue @this)
        {
            return (@this.Type == JsCsGlueType.Basic);
        }

        public static bool IsBasicNotNull(this IJsCsGlue @this)
        {
            return (@this.CValue != null && @this.Type == JsCsGlueType.Basic);
        }

        public static ISet<IJsCsGlue> GetAllChildren(this IJsCsGlue @this, bool includeMySelf = false)
        {
            var res = new HashSet<IJsCsGlue>();
            if (includeMySelf)
                res.Add(@this);

            @this.AppendAllChildren(res);
            return res;
        }

        public static void AppendAllChildren(this IJsCsGlue @this, ISet<IJsCsGlue> res)
        {
            var children = @this.Children;
            if (children == null)
                return;

            foreach (var child in children)
            {
                if (res.Add(child))
                {
                    child.AppendAllChildren(res);
                }
            }
        }

        public static void VisitAllChildren(this IJsCsGlue @this, Func<IJsCsGlue, bool> visit)
        {
            if (!visit(@this))
                return;

            var res = new HashSet<IJsCsGlue> {@this};
            @this.VisitAllChildren(visit, res);
        }

        public static void VisitAllChildren(this IJsCsGlue @this, Func<IJsCsGlue, bool> visit, ISet<IJsCsGlue> res)
        {
            var children = @this.Children;
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

        public static void UnsafeVisitAllChildren(this IJsCsGlue @this, Func<IJsCsGlue, bool> visit, bool check=true)
        {
            if (check && !visit(@this))
                return;

            var children = @this.Children;
            if (children == null)
                return;

            foreach (var child in children)
            {
                if (visit(child))
                {
                    child.UnsafeVisitAllChildren(visit, false);
                }
            }
        }

        public static ISet<IJsCsGlue> GetAllChildren(this IJsCsGlue @this, Func<JsCsGlueType, bool> filter)
        {
            var res = new HashSet<IJsCsGlue>();
            if (filter(@this.Type))
                return res;
       
            res.Add(@this);
            @this.AppendAllChildren(filter, res);
            return res;
        }

        public static void AppendAllChildren(this IJsCsGlue @this, Func<JsCsGlueType, bool> filter, ISet<IJsCsGlue> res)
        {
            var children = @this.Children;
            if (children == null)
                return;

            foreach (var child in children)
            {
                if (!filter(child.Type) && res.Add(child))
                {
                    child.AppendAllChildren(filter, res);
                }
            }
        }

        public static IJavascriptObject GetJsSessionValue(this IJsCsGlue @this)
        {
            var inj = @this as IJsCsMappedBridge;
            return (inj != null) ? inj.CachableJsValue : @this.JsValue;
        }
    }
}
