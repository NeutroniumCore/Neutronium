using System;
using System.Threading.Tasks;

namespace  Neutronium.Core.Navigation
{
    public interface INavigationSolver : IDisposable
    {
        bool UseINavigable { get; set; }

        Task<IHTMLBinding> NavigateAsync(object viewModel, string id = null, JavascriptBindingMode mode = JavascriptBindingMode.TwoWay);

        event EventHandler<NavigationEvent> OnNavigate;

        event EventHandler<DisplayEvent> OnDisplay;

        event EventHandler OnFirstLoad;
    }
}
