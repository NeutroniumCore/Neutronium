using System;

namespace Neutronium.Core.Binding.Updaters
{
    internal interface IContextsManager
    {
        void CheckUiContext();
        void DispatchInJavascriptContext(Action action);
    }
}
