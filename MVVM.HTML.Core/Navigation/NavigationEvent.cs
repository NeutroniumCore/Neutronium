using System;

namespace MVVM.HTML.Core.Navigation
{
    public class NavigationEvent : EventArgs
    {
        public object NewViewModel { get; private set; }

        public object OldViewModel { get; private set; }

        public NavigationEvent(object newViewModel, object oldViewModel)
        {
            NewViewModel = newViewModel;
            OldViewModel = oldViewModel;
        }
    }
}
