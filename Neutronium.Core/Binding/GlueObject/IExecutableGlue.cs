using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IExecutableGlue
    {
        void UpdateJsObject(IJavascriptObject javascriptObject);

        void Execute(IJavascriptObject[] parameters);
    }
}
