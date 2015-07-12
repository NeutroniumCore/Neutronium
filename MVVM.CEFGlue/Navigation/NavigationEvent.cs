using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.CEFGlue
{
    public class NavigationEvent : EventArgs
    {
        public object NewViewModel { get; private set; }

        public object OldViewModel { get; private set; }

        public NavigationEvent(object iNewViewModel, object iOldViewModel)
        {
            NewViewModel = iNewViewModel;
            OldViewModel = iOldViewModel;
        }
    }
}
