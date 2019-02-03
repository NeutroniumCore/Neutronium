using Example.Cfx.Spa.Routing.ViewModel;
using System;

namespace Example.Cfx.Spa.Routing
{
    public class RouterSolver : IRouterSolver
    {
        public string SolveRoute(Type type)
        {
            return type == typeof(MainViewModel) ? "main" : "about";
        }

        public Type SolveType(string route)
        {
            return (route == "main") ? typeof(MainViewModel) : typeof(AboutViewModel);
        }
    }
}
