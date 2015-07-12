using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using MVVM.CEFGlue.Infra.VM;
using MVVM.CEFGlue.Navigation;
using MVVM.CEFGlue.CefSession;


namespace MVVM.CEFGlue
{
    public partial class HTMLWindow : HTMLControlBase, INavigationSolver, IWebViewLifeCycleManager, IDisposable
    {
        public HTMLWindow(): this(new NavigationBuilder())
        {
        }

        public HTMLWindow(IUrlSolver iIUrlSolver): base(iIUrlSolver)
        {
            _INavigationBuilder = iIUrlSolver as INavigationBuilder;
        }

        private INavigationBuilder _INavigationBuilder;
        public INavigationBuilder NavigationBuilder
        {
            get { return _INavigationBuilder; }
        }

        public Task NavigateAsync(object iViewModel, string Id = null, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay)
        {
            return NavigateAsyncBase(iViewModel, Id, iMode);
        }

    }
}