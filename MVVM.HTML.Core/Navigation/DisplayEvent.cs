using System;

namespace MVVM.HTML.Core.Navigation
{
    public class DisplayEvent : EventArgs
    {
        public object DisplayedViewModel { get; private set; }

        public DisplayEvent(object iNewViewModel)
        {
            DisplayedViewModel = iNewViewModel;
        }
    }
}
