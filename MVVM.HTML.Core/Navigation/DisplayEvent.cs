using System;

namespace MVVM.HTML.Core
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
