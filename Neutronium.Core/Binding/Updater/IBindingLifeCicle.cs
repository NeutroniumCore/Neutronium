using System;

namespace Neutronium.Core.Binding.Updater
{
    internal interface IBindingLifeCicle
    {
        event EventHandler<EventArgs> OnJavascriptSessionReady;
    }
}
