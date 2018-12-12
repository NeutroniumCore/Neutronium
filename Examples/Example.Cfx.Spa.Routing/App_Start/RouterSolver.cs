using System;
using Example.Cfx.Spa.Routing.ViewModel;

namespace Example.Cfx.Spa.Routing.App_Start {
    public class RouterSolver : IRouterSolver {
        public string SolveRoute(Type type) {
            return type == typeof(MainViewModel) ? "Main" : "About";
        }

        public Type SolveType(string route) {
            return (route == "Main") ? typeof(MainViewModel) : typeof(AboutViewModel);
        }
    }
}
