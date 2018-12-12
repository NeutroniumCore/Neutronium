using System.Threading.Tasks;

namespace Example.Cfx.Spa.Routing {
    public class RouteContext {
        public object ViewModel { get; private set; }
        public string Route { get; private set; }
        public Task Task => _TaskCompletionSource.Task;

        private readonly TaskCompletionSource<int> _TaskCompletionSource = new TaskCompletionSource<int>();

        public RouteContext(object viewModel, string route) {
            ViewModel = viewModel;
            Route = route;
        }

        public void Complete() {
            _TaskCompletionSource.TrySetResult(0);
        }

        public void Redirect(string redirect, object viewModel) {
            Route = redirect;
            ViewModel = viewModel;
        }
    }
}