using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.Updaters
{
    internal interface IJavascriptUpdater
    {
        void OnUiContext(ObjectChangesListener off);

        bool NeedToRunOnJsContext { get; }

        void OnJsContext();
    }
}
