using Example.Cfx.Spa.Routing.ViewModel;
using Neutronium.MVVMComponents;

namespace Example.Cfx.Spa.Routing {
    public class ApplicationViewModel {
        public ApplicationInformation ApplicationInformation { get; } = new ApplicationInformation();
        public IWindowViewModel Window { get; }
        public NavigationViewModel Router { get; }

        public object CurrentViewModel { get; set; }

        public ApplicationViewModel(IWindowViewModel window, NavigationViewModel router) {
            Window = window;
            Router = router;
        }
    }
}