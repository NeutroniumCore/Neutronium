using System.Threading.Tasks;
using Neutronium.Core;
using Neutronium.Core.Navigation;

namespace Neutronium.WPF
{
    public class HTMLWindow : Neutronium.WPF.Internal.HTMLControlBase, INavigationSolver
    {
        public HTMLWindow() : this(new NavigationBuilder())
        {
        }

        public HTMLWindow(IUrlSolver urlSolver) : base(urlSolver)
        {
            NavigationBuilder = urlSolver as INavigationBuilder;
        }
      
        public INavigationBuilder NavigationBuilder { get; }

        public async Task<IHTMLBinding> NavigateAsync(object iViewModel, string id = null, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay)
        {
            return await NavigateAsyncBase(iViewModel, id, iMode);
        }
    }
}
