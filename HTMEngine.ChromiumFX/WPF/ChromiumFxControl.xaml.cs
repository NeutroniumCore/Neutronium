using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HTML_WPF.Component;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Infra.VM;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.Window;
using MVVM.HTML.Core.JavascriptUIFramework;
using MVVM.HTML.Core.Navigation;

namespace HTMEngine.ChromiumFX.WPF
{
    public partial class ChromiumFxControl 
    {
        protected ChromiumFxControl(IUrlSolver iIUrlSolver) 
        {
            InitializeComponent();
        }
    }
}
