using System;
using Neutronium.Core.Binding;
using Neutronium.Core.Binding.Converter;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Extension
{
    public static class JavascriptToCSharpConverterExtension
    {
        public static MayBe<T> GetFirstArgument<T>(this IJavascriptToCSharpConverter converter, IJavascriptObject[] javascriptObjects)
        {
            if (javascriptObjects.Length == 0)
                return new MayBe<T>();

            try
            {
                var found = converter.GetCachedOrCreateBasic(javascriptObjects[0], typeof(T));
                return (found == null)?  new MayBe<T>(): new MayBe<T>(found.CValue);
            }
            catch (Exception)
            {
                return new MayBe<T>();
            }
        }

        public static object GetFirstArgumentOrNull(this IJavascriptToCSharpConverter converter, IJavascriptObject[] javascriptObjects)
        {
            return javascriptObjects.Length == 0 ? null : converter.GetArgument(javascriptObjects[0], null);
        }

        private static object GetArgument(this IJavascriptToCSharpConverter converter, IJavascriptObject javascriptObject, Type targetType)
        {
            try
            {
                var found = converter.GetCachedOrCreateBasic(javascriptObject, targetType);
                return found?.CValue;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
