using System;
using System.Threading.Tasks;

using MVVM.HTML.Core;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Navigation;


namespace HTML_WPF.Component
{
    public class HTMLWindow : HTMLControlBase, INavigationSolver, IWebViewLifeCycleManager, IDisposable
    {
        private readonly INavigationBuilder _INavigationBuilder;

        public HTMLWindow() : this(new NavigationBuilder())
        {
        }

        public HTMLWindow(IUrlSolver iIUrlSolver) : base(iIUrlSolver)
        {
            _INavigationBuilder = iIUrlSolver as INavigationBuilder;
        }
      
        public INavigationBuilder NavigationBuilder
        {
            get { return _INavigationBuilder; }
        }

        public async Task NavigateAsync(object iViewModel, string Id = null, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay)
        {
            await NavigateAsyncBase(iViewModel, Id, iMode);
        }
    }
}
