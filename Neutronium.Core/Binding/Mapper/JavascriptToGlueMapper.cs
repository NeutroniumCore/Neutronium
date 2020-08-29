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

        public IGlueConvertible GetGlueConvertible(IJavascriptObject javascriptObject, Type targetType)
        {
            if (javascriptObject == null)
                return new RawGlueConvertible(null);

            if (_JsUpdateHelper.GetSimpleValue(javascriptObject, out var targetValue, targetType))
                return new RawGlueConvertible(targetValue);

            if (targetType?.IsEnum == true)
            {
                var intValue = javascriptObject.GetValue("intValue")?.GetIntValue();
                return new RawGlueConvertible(intValue.HasValue ? Enum.ToObject(targetType, intValue.Value) : null);
            }

            var res = _SessionCache.GetCached(javascriptObject);
            if (res != null)
                return new SolvedGlueConvertible(res);

            var message = $"Unable to convert javascript object: {javascriptObject} to C# session. Value will be default to null. Please check javascript bindings.";
            _JsUpdateHelper.Logger.Info(message);
            throw new ArgumentException(message);
        }
    }
}
