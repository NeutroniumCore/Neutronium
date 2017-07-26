using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Listeners
{
    internal interface IExitContext
    {
        void AddToUnlisten(IJavascriptObject exiting);
    }
}