using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Binding.Extension
{
    public static class IJavascriptToCSharpConverterExtension
    {

        public static object GetArgument(this IJavascriptToCSharpConverter converter, IJavascriptObject javascriptObject)
        {
            var found = converter.GetCachedOrCreateBasic(javascriptObject, null);
            return (found != null) ? found.CValue : null;
        }


        public static object GetFirstArgumentOrNull(this IJavascriptToCSharpConverter converter, IJavascriptObject[] javascriptObjects)
        {
            if (javascriptObjects.Length == 0)
                return null;

            return converter.GetArgument(javascriptObjects[0]);
        }
    }
}
