using System;

namespace Neutronium.Core.WebBrowserEngine.Control
{
    public class DebugEventArgs: EventArgs 
    {
        public bool Opening { get; }

        public DebugEventArgs(bool opening) 
        {
            Opening = opening;
        }
    }
}
