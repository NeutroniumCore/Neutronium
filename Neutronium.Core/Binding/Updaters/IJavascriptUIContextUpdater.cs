using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.Updaters
{
    internal interface IJavascriptUIContextUpdater
    {
        IJavascriptJsContextUpdater ExecuteOnUiContext(ObjectChangesListener off);
    }
}
