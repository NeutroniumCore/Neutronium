using System;

namespace Neutronium.Core.WebBrowserEngine.Window
{
    public class ContextMenuItem
    {
        public Action Command { get; }
        public string Name { get; }
        public bool Enabled { get; set; }

        public ContextMenuItem(Action command, string name, bool enabled=true)
        {
            Command = command;
            Name = name;
            Enabled = enabled;
        }
    }
}
