using System;

namespace Neutronium.Core.WebBrowserEngine.Window
{
    public class ContextMenuItem
    {
        public Action Command { get; }
        public string Name { get; }

        public ContextMenuItem(Action command, string name)
        {
            Command = command;
            Name = name;
        }
    }
}
