using System;

namespace Example.Cfx.Spa.Routing
{
    public interface IRouterSolver
    {
        string SolveRoute(Type type);

        Type SolveType(string route);
    }
}
