using System;

namespace Example.Cfx.Spa.Routing.App_Start {
    public class RoutedEventArgs : EventArgs {
        public RoutedEventArgs(RouteContext routeContext) : this(new RouteInfo(routeContext)) {
        }

        public RoutedEventArgs(object viewModel, string routeName) {
            NewRoute = new RouteInfo(viewModel, routeName);
        }

        public RoutedEventArgs(RouteInfo route) {
            NewRoute = route;
        }

        public RouteInfo NewRoute { get; }
    }
}
