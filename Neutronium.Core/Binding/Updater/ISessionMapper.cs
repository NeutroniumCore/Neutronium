using System;

namespace Neutronium.Core.Binding.Updater
{
    internal interface ISessionMapper
    {
        event EventHandler<EventArgs> OnJavascriptSessionReady;
    }
}
