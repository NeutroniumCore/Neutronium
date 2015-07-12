using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.CEFGlue.Navigation.Window
{
    public class WindowHelper
    {
        public HTMLLogicWindow __window__ { get;private set;}

        public WindowHelper(HTMLLogicWindow iwindow)
        {
            __window__ = iwindow;
        }
    }
}
