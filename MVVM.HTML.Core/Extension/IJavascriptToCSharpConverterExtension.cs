using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using System;

namespace MVVM.HTML.Core.Extension
{
    public static class IJavascriptToCSharpConverterExtension
    {
        public static object GetFirstArgumentOrNull(this IJavascriptToCSharpConverter converter, IJavascriptObject[] javascriptObjects) 
        {
            return javascriptObjects.Length == 0 ? null : converter.GetArgument(javascriptObjects[0]);
        }

        private static object GetArgument(this IJavascriptToCSharpConverter converter, IJavascriptObject javascriptObject)
        {
            try
            {
                var found = converter.GetCachedOrCreateBasic(javascriptObject, null);
                return found?.CValue;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
