using System;
using Neutronium.Core.Binding.Updater;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Mapper
{
    internal class JavascriptToGlueMapper : IJavascriptToGlueMapper
    {
        private readonly IJsUpdateHelper _JsUpdateHelper;
        private readonly ISessionCache _SessionCache;

        public JavascriptToGlueMapper(IJsUpdateHelper jsUpdateHelper, ISessionCache sessionCache)
        {
            _JsUpdateHelper = jsUpdateHelper;
            _SessionCache = sessionCache;
        }

        public IGlueMapable GetGlueConvertible(IJavascriptObject javascriptObject, Type targetType)
        {
            if (javascriptObject == null)
                return new RawGlueMapable(null, null);
            
            if (_JsUpdateHelper.GetSimpleValue(javascriptObject, out var targetValue, targetType))
                return new RawGlueMapable(targetValue, javascriptObject);

            if (targetType?.IsEnum == true)
            {
                var intValue = javascriptObject.GetValue("intValue")?.GetIntValue();
                return new RawGlueMapable(intValue.HasValue ? Enum.ToObject(targetType, intValue.Value) : null, javascriptObject);
            }

            var res = _SessionCache.GetCached(javascriptObject);
            if (res != null)
                return new SolvedGlueMapable(res);

            var message = $"Unable to convert javascript object: {javascriptObject} to C# session. Value will be default to null. Please check javascript bindings.";
            _JsUpdateHelper.Logger.Info(message);
            throw new ArgumentException(message);
        }
    }
}
