using System;
using Chromium.Remote;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.WebBrowserEngine.ChromiumFx.Convertion 
{
    public static class CfrV8HandlerHelper 
    {
        public static CfrV8Handler Convert(this Action<string, IJavascriptObject, IJavascriptObject[]> function, string name)
        {
            var res = new CfrV8Handler();
            res.Execute += (o, e) => function(name, e.Object.Convert(), e.Arguments.Convert());
            return res;
        }

        public static CfrV8Handler Convert(this Action<IJavascriptObject[]> function)
        {
            var res = new CfrV8Handler();
            res.Execute += (o, e) => function(e.Arguments.Convert());
            return res;
        }

        public static CfrV8Handler Convert(this Action<IJavascriptObject> function)
        {
            var res = new CfrV8Handler();
            res.Execute += (o, e) => function(e.Arguments[0].Convert());
            return res;
        }

        public static CfrV8Handler Convert(this Action<IJavascriptObject, IJavascriptObject> function)
        {
            var res = new CfrV8Handler();
            res.Execute += (o, e) =>  function(e.Arguments[0].Convert(), e.Arguments[1].Convert());
            return res;
        }

        public static CfrV8Handler Convert(this Action<IJavascriptObject, IJavascriptObject, IJavascriptObject> function)
        {
            var res = new CfrV8Handler();
            res.Execute += (o, e) => function(e.Arguments[0].Convert(), e.Arguments[1].Convert(), e.Arguments[2].Convert());
            return res;
        }

        public static CfrV8Handler Convert(this Action<IJavascriptObject, IJavascriptObject, IJavascriptObject, IJavascriptObject> function)
        {
            var res = new CfrV8Handler();
            res.Execute += (o, e) => function(e.Arguments[0].Convert(), e.Arguments[1].Convert(), e.Arguments[2].Convert(), e.Arguments[3].Convert());
            return res;
        }
    }
}
