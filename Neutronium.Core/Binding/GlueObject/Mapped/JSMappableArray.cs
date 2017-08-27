using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal class JSMappableArray: JsArray, IJsCsMappedBridge
    {
        private IJavascriptObject _MappedJSValue;

        public override IJavascriptObject CachableJsValue => _MappedJSValue;

        public void SetMappedJSValue(IJavascriptObject jsobject)
        {
            _MappedJSValue = jsobject;
        }

        public JSMappableArray(List<IJsCsGlue> values, IEnumerable collection, Type individual):
            base(values, collection, individual)
        {
        }
    }
}
