using HTML_WPF.Component;
using MVVM.Awesomium;
using MVVM.Cef.Glue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MVVM_Awesonium_UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            HTMLEngineFactory.Engine.Register(new AwesomiumWPFWebWindowFactory() );
            HTMLEngineFactory.Engine.Register(new CefGlueWPFWebWindowFactory());
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            HTMLEngineFactory.Engine.Dispose();
            base.OnExit(e);
        }
    }
}
