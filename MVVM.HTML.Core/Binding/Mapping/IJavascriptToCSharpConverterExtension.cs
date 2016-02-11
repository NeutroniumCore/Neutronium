using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.V8JavascriptObject;

namespace MVVM.HTML.Core.Binding
{
    public static class IJavascriptToCSharpConverterExtension
    {
        private static object Convert(this IJavascriptToCSharpConverter converter, IJavascriptObject value)
        {
            var found = converter.GetCachedOrCreateBasic(value, null);
            return (found != null) ? found.CValue : null;
        }

        public static object GetArguments(this IJavascriptToCSharpConverter converter, IJavascriptObject[] e)
        {
            return (e.Length == 0) ? null : converter.Convert(e[0]);
        }
    }
}
