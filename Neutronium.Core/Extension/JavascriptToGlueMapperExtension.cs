using System;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Extension
{
    public static class JavascriptToGlueMapperExtension
    {
        internal static MayBe<T> GetFirstArgument<T>(this IJavascriptToGlueMapper converter, IJavascriptObject[] javascriptObjects)
        {
            if (javascriptObjects.Length == 0)
                return new MayBe<T>();

            var argument = javascriptObjects[0];
            if (argument == null)
                return new MayBe<T>();

            try
            {
                var found = converter.GetGlueConvertible(argument, typeof(T));
                return new MayBe<T>(found.Source);
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
                return converter.GetGlueConvertible(javascriptObject, targetType).Source;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
