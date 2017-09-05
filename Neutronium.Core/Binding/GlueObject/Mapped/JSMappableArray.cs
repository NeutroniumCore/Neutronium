using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal class JsMappableArray: JsArray, IJsCsMappedBridge
    {
        private IJavascriptObject _MappedJsValue;

        public override IJavascriptObject CachableJsValue => _MappedJsValue;

        public void SetMappedJsValue(IJavascriptObject jsobject)
        {
            _MappedJsValue = jsobject;
        }

        public JsMappableArray(IEnumerable collection, Type individual):
            base(collection, individual)
        {
        }
    }
}
