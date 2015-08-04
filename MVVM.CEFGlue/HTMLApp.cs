using MVVM.CEFGlue.CefSession;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xilium.CefGlue;
using Xilium.CefGlue.WPF;

namespace MVVM.CEFGlue
{
    public class HTMLApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var settings = GetCefSettings();

            if (CefCoreSessionSingleton.Session == null)
            {
                CefCoreSessionSingleton.GetAndInitIfNeeded(new WPFUIDispatcher(this.Dispatcher), settings);
            }
            else
            {
                Trace.WriteLine("MVVM for CEFGlue: Impossible to load custo settings");
            }

            base.OnStartup(e);
        }

        protected virtual CefSettings GetCefSettings()
        {
            return null;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            CefCoreSessionSingleton.Clean();
        }
    }
}
