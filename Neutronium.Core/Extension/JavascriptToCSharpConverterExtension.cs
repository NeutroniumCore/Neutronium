using System;
using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Extension
{
    public static class JavascriptToCSharpConverterExtension
    {
        public static Result<T> GetFirstArgument<T>(this IJavascriptToCSharpConverter converter, IJavascriptObject[] javascriptObjects)
        {
            if (javascriptObjects.Length == 0)
                return new Result<T>();

            try 
            {
                var found = converter.GetCachedOrCreateBasic(javascriptObjects[0], typeof(T));
                return new Result<T>(found);
            }
            catch (Exception) 
            {
                return new Result<T>();
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
