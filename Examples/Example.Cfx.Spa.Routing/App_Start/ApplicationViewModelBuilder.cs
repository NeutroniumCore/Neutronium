using System.Windows;
using Example.Cfx.Spa.Routing.App_Start;
using Neutronium.WPF.ViewModel;

namespace Example.Cfx.Spa.Routing {
    public class ApplicationViewModelBuilder {
        public ApplicationViewModel ApplicationViewModel { get; }

        public ApplicationViewModelBuilder(Window wpfWindow) {
            var window = new WindowViewModel(wpfWindow);
            var routeSolver = new RouterSolver();
            ApplicationViewModel = new ApplicationViewModel(window, new NavigationViewModel(routeSolver));
        }
    }
}
