using System;
using System.Threading.Tasks;
using MVVM.HTML.Core.Navigation;

namespace  MVVM.HTML.Core
{
    public interface INavigationSolver : IDisposable
    {
        bool UseINavigable { get; set; }

        Task NavigateAsync(object iViewModel, string Id = null, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay);

        event EventHandler<NavigationEvent> OnNavigate;

        event EventHandler<DisplayEvent> OnDisplay;

        event EventHandler OnFirstLoad;
    }
}
