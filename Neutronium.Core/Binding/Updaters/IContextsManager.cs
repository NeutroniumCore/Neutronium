using System;

namespace Neutronium.Core.Binding.Updaters
{
    internal interface IContextsManager
    {
        bool isInUiContext { get; }
        void CheckUiContext();     
        void DispatchInJavascriptContext(Action action);
        void DispatchInUiContextBindingPriority(Action action);
    }
}
