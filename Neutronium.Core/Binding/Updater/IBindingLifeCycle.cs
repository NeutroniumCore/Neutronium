using System;

namespace Neutronium.Core.Binding.Updater
{
    internal interface IBindingLifeCycle
    {
        event EventHandler<EventArgs> OnJavascriptSessionReady;
    }
}
