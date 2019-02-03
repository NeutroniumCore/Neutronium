using System.ComponentModel;

namespace Example.Cfx.Spa.Routing
{
    public class RoutingEventArgs : CancelEventArgs
    {
        public RoutingEventArgs(RouteContext toContext, string fromRoute, object fromVm)
        {
            To = new RouteInfo(toContext);
            From = new RouteInfo(fromVm, fromRoute);
        }

        public RouteInfo To { get; }
        public RouteInfo From { get; }
        public string RedirectedTo { get; private set; }

        public void RedirectToroute(string newRouteName)
        {
            if (To.RouteName == newRouteName)
                return;
            Cancel = From.RouteName == newRouteName;
            RedirectedTo = newRouteName;
        }
    }

    public struct RouteInfo
    {
        public object ViewModel { get; }
        public string RouteName { get; }

        public RouteInfo(RouteContext routeContext) : this(routeContext.ViewModel, routeContext.Route)
        {
        }

        public RouteInfo(object viewModel, string routeName)
        {
            ViewModel = viewModel;
            RouteName = routeName;
        }
    }
}