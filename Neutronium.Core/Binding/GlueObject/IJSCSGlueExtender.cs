using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;

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

        public static ISet<IJSCSGlue> GetAllChildren(this IJSCSGlue @this, bool includeMySelf = false)
        {
            var res = new HashSet<IJSCSGlue>();
            if (includeMySelf)
                res.Add(@this);

            @this.GetAllChildren(res);
            return res;
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
