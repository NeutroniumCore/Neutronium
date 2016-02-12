using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.V8JavascriptObject;

namespace MVVM.HTML.Core.Binding.Extension
{
    public static class IJavascriptToCSharpConverterExtension
    {
        public static object GetArguments(this IJavascriptToCSharpConverter converter, IJavascriptObject[] e)
        {
            if (e.Length == 0)
                return null;

            var found = converter.GetCachedOrCreateBasic(e[0], null);
            return (found != null) ? found.CValue : null;
        }
    }
}
