using System;
using Chromium.Remote;
using Chromium.Remote.Event;
using HTMEngine.ChromiumFX.EngineBinding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace HTMEngine.ChromiumFX.Convertion 
{
    public static class CfrV8HandlerGHelper 
    {
        public static CfrV8Handler Convert(this Action<string, IJavascriptObject, IJavascriptObject[]> function, string name, CfrFrame frame) {
            var res = new CfrV8Handler();
            //var browser = frame.Browser;
            //Action<CfrV8HandlerExecuteEventArgs> execute = (e) => 
            //{
            //    using (new ChromiumFXCRemoteContext(browser)) 
            //    {
            //        function(name, e.Object.Convert(), e.Arguments.Convert());
            //    }
            //};

            //res.Execute += (o, e) => execute(e);
            res.Execute += (o, e) => function(name, e.Object.Convert(), e.Arguments.Convert());
            return res;
        }
    }
}
