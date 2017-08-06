using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    internal interface IExecutableGlue
    {
        void UpdateJsObject(IJavascriptObject javascriptObject);
        void Execute(IJavascriptObject[] parameters);
    }
}
