using MVVM.CEFGlue.CefSession;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xilium.CefGlue;

namespace MVVM.CEFGlue
{
    public class HTMLApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var settings = GetCefSettings();
            if (settings==null)
            {
                if (CefCoreSessionSingleton.Session==null)
                {
                    CefCoreSessionSingleton.GetAndInitIfNeeded(settings);
                }
                else
                {
                    Trace.WriteLine("MVVM for CEFGlue: Impossible to load custo settings");
                }
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
