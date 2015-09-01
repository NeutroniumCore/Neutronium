using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Xilium.CefGlue;

using MVVM.Cef.Glue.CefSession;
using MVVM.Cef.Glue.WPF;
using HTML_WPF.Component;

namespace MVVM.Cef.Glue
{
    public class HTMLApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            HTMLEngineFactory.Engine.Register(new CefGlueWPFWebWindowFactory(GetCefSettings()));

            base.OnStartup(e);
        }

        protected virtual CefSettings GetCefSettings()
        {
            return null;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            HTMLEngineFactory.Engine.Dispose();
        }
    }
}
