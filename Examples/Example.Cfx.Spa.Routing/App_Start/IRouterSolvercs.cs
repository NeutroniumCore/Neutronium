using System;

namespace Example.Cfx.Spa.Routing.App_Start {
    public interface IRouterSolver {
        string SolveRoute(Type type);

        Type SolveType(string route);
    }
}
