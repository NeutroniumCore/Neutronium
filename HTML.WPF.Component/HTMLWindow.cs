using System;
using System.Threading.Tasks;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Navigation;

namespace HTML_WPF.Component
{
    public class HTMLWindow : HTMLControlBase, INavigationSolver, IWebViewLifeCycleManager, IDisposable
    {
        public HTMLWindow() : this(new NavigationBuilder())
        {
        }

        public HTMLWindow(IUrlSolver iIUrlSolver) : base(iIUrlSolver)
        {
            NavigationBuilder = iIUrlSolver as INavigationBuilder;
        }
      
        public INavigationBuilder NavigationBuilder { get; private set; }

        public async Task NavigateAsync(object iViewModel, string Id = null, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay)
        {
            await NavigateAsyncBase(iViewModel, Id, iMode);
        }
    }
}
