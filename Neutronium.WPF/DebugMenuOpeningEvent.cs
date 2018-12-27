using System;
using System.Collections.Generic;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.WPF
{
    public class DebugMenuOpeningEvent: EventArgs
    {
        public List<ContextMenuItem> AdditionalMenuItems { get; } = new List<ContextMenuItem>();
    }
}
