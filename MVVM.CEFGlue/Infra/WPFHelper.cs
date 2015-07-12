using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVVM.CEFGlue.Infra
{
    public static class WPFHelper
    {
        public static void RunOnUIThread(Action act)
        {
            Application.Current.Dispatcher.BeginInvoke(act);
        }

        public static void SyncRunOnUIThread(Action act)
        {
            Application.Current.Dispatcher.Invoke(act);
        }
    }
}
