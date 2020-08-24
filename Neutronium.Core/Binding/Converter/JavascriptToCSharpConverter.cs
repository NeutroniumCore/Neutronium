using System;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Basic;
using Neutronium.Core.Binding.Updater;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Converter
{
    internal class JavascriptToCSharpConverter : IJavascriptToCSharpConverter
    {
        private readonly IJsUpdateHelper _JsUpdateHelper;
        private readonly SessionCacher _SessionCache;

        public JavascriptToCSharpConverter(IJsUpdateHelper jsUpdateHelper, SessionCacher sessionCache)
        {
            _JsUpdateHelper = jsUpdateHelper;
            _SessionCache = sessionCache;
        }

        public IJsCsGlue GetCachedOrCreateBasic(IJavascriptObject javascriptObject, Type targetType)
        {
            if (javascriptObject == null)
                return null;

            if (_JsUpdateHelper.GetSimpleValue(javascriptObject, out var targetValue, targetType))
                return new JsBasicObject(javascriptObject, targetValue);

            if (targetType?.IsEnum == true)
            {
                var intValue = javascriptObject.GetValue("intValue")?.GetIntValue();
                if (!intValue.HasValue)
                    return null;

                targetValue = Enum.ToObject(targetType, intValue.Value);
                return new JsEnum(javascriptObject, (Enum)targetValue);
            }

            var res = _SessionCache.GetCached(javascriptObject);
            if (res != null)
                return res;

            var message = $"Unable to convert javascript object: {javascriptObject} to C# session. Value will be default to null. Please check javascript bindings.";
            _JsUpdateHelper.Logger.Info(message);
            throw new ArgumentException(message);
        }
    }
}
