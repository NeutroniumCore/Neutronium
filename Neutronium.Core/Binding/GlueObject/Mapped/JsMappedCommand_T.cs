﻿using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal sealed class JsMappedCommand<T>: JsCommand<T>, IJsCsMappedBridge
    {
        private IJavascriptObject _MappedJsValue;

        public override IJavascriptObject CachableJsValue => _MappedJsValue;

        public override void SetJsValue(IJavascriptObject value, ISessionCache sessionCache)
        {
            SetJsValue(value);
        }

        public void SetMappedJsValue(IJavascriptObject jsobject)
        {
            _MappedJsValue = jsobject;
            UpdateJsObject(_MappedJsValue);
        }

        public JsMappedCommand(HtmlViewContext context, IJavascriptToGlueMapper converter, ICommand<T> resultCommand)
            :base(context, converter, resultCommand)
        {
        }
    }
}
