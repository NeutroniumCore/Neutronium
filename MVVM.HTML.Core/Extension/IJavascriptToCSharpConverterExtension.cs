using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Extension
{
    public static class IJavascriptToCSharpConverterExtension
    {

        public static object GetArgument(this IJavascriptToCSharpConverter converter, IJavascriptObject javascriptObject)
        {
            var found = converter.GetCachedOrCreateBasic(javascriptObject, null);
            return found?.CValue;
        }


        public static object GetFirstArgumentOrNull(this IJavascriptToCSharpConverter converter, IJavascriptObject[] javascriptObjects) 
        {
            return javascriptObjects.Length == 0 ? null : converter.GetArgument(javascriptObjects[0]);
        }
    }
}
