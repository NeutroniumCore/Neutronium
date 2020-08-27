using System;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Extension
{
    public static class JavascriptToCSharpConverterExtension
    {
        internal static MayBe<T> GetFirstArgument<T>(this IJavascriptToGlueMapper converter, IJavascriptObject[] javascriptObjects)
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

        internal static object GetFirstArgumentOrNull(this IJavascriptToGlueMapper converter, IJavascriptObject[] javascriptObjects)
        {
            return javascriptObjects.Length == 0 ? null : converter.GetArgument(javascriptObjects[0], null);
        }

        private static object GetArgument(this IJavascriptToGlueMapper converter, IJavascriptObject javascriptObject, Type targetType)
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
