using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.HTML.Core;
using MVVM.HTML.Core.Navigation;


namespace HTML_WPF.Component
{
    public partial class HTMLWindow : HTMLControlBase, INavigationSolver, IWebViewLifeCycleManager, IDisposable
    {
        public HTMLWindow() : this(new NavigationBuilder())
        {
        }

        public HTMLWindow(IUrlSolver iIUrlSolver)
            : base(iIUrlSolver)
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
