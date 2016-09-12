using System;

namespace Neutronium.Core.Navigation
{
    public class DisplayEvent : EventArgs
    {
        public object DisplayedViewModel { get; }

        public DisplayEvent(object iNewViewModel)
        {
            DisplayedViewModel = iNewViewModel;
        }
    }
}
