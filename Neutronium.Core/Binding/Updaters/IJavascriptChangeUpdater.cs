namespace Neutronium.Core.Binding.Updaters 
{
    internal interface IJavascriptUpdater
    {
        void OnUiContext();

        bool NeedToRunOnJsContext { get; }

        void OnJsContext();
    }
}
