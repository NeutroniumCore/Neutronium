using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal interface IExecutableGlue
    {
        void Execute(params IJavascriptObject[] parameters);
    }
}
