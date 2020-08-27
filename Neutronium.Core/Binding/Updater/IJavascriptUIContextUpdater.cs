using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.Updater
{
    internal interface IJavascriptUIContextUpdater
    {
        IJavascriptJsContextUpdater ExecuteOnUiContext(ObjectChangesListener off);
    }
}
