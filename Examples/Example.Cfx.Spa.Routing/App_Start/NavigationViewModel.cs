using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Example.Cfx.Spa.Routing.App_Start;
using Neutronium.Example.ViewModel.Infra;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;

namespace Example.Cfx.Spa.Routing {
    public class NavigationViewModel: ViewModelBase {
        public IResultCommand<string, BeforeRouterResult> BeforeResolveCommand { get; }
        public ISimpleCommand<string> AfterResolveCommand { get; }

        private string _Route;
        public string Route {
            get => _Route;
            private set => Set(ref _Route, value, nameof(Route));
        }

        public event EventHandler<RoutingEventArgs> OnNavigating;
        public event EventHandler<RoutedEventArgs> OnNavigated;

        private readonly IRouterSolver _RouterSolver;
        private readonly Queue<RouteContext> _CurrentNavigations = new Queue<RouteContext>();

        private object _ViewModel;

        public NavigationViewModel(IRouterSolver routerSolver) {
            _RouterSolver = routerSolver;
            AfterResolveCommand = new RelaySimpleCommand<string>(AfterResolve);
            BeforeResolveCommand = RelayResultCommand.Create<string, BeforeRouterResult>(BeforeResolve);
        }

        public static NavigationViewModel Create(IRouterSolver routerSolver)
            => new NavigationViewModel(routerSolver);

        private BeforeRouterResult BeforeResolve(string routeName) {
            var context = GetRouteContext(routeName);
            return (context == null) ? BeforeRouterResult.Cancel() : Navigate(context);
        }

        private BeforeRouterResult Navigate(RouteContext to) {
            var routingEventArgs = new RoutingEventArgs(to, Route, _ViewModel);
            OnNavigating?.Invoke(this, routingEventArgs);

            if (routingEventArgs.Cancel) {
                _CurrentNavigations.Dequeue();
                to.Complete();
                return BeforeRouterResult.Cancel();
            }

            var redirect = routingEventArgs.RedirectedTo;
            if (string.IsNullOrEmpty(redirect)) {
                _ViewModel = to.ViewModel;
                return BeforeRouterResult.Ok(_ViewModel);
            }

            to.Redirect(redirect, GetViewModelFromRoute(redirect));
            return BeforeRouterResult.CreateRedirect(redirect);
        }

        private RouteContext GetRouteContext(string routeName) {
            if (_CurrentNavigations.Count == 0)
                return CreateRouteContext(routeName);

            var context = _CurrentNavigations.Peek();
            if (context.Route != routeName) {
                Console.WriteLine($"Navigation inconsistency: from browser {routeName}, from context: {context.Route}");
                return null;
            }

            return context;
        }

        private RouteContext CreateRouteContext(string routeName) {
            return CreateRouteContext(GetViewModelFromRoute(routeName), routeName);
        }

        private object GetViewModelFromRoute(string routeName) {
            var type = _RouterSolver.SolveType(routeName);
            return Activator.CreateInstance(type);
        }

        private RouteContext CreateRouteContext(object viewModel, string routeName) {
            var routeContext = new RouteContext(viewModel, routeName);
            _CurrentNavigations.Enqueue(routeContext);
            return routeContext;
        }

        private void AfterResolve(string routeName) {
            var context = _CurrentNavigations.Dequeue();
            if (context.Route != routeName) {
                Console.WriteLine($"Navigation inconsistency: from browser {routeName}, from context: {context.Route}. Maybe rerouted?");
            }
            context.Complete();

            Route = routeName;
            OnNavigated?.Invoke(this, new RoutedEventArgs(context));
        }

        public Task Navigate(object viewModel, string routeName) {
            var route = routeName ?? _RouterSolver.SolveRoute(viewModel.GetType());

            if (Route == route) {
                if (!ReferenceEquals(_ViewModel, viewModel))
                    OnNavigated?.Invoke(this, new RoutedEventArgs(viewModel, route));

                _ViewModel = viewModel;
                return Task.FromResult(0);
            }

            var routeContext = CreateRouteContext(viewModel, route);
            Route = route;
            return routeContext.Task;
        }

        public async Task Navigate<T>(NavigationContext<T> context = null) {
            var resolutionKey = context?.ResolutionKey;
            var vm = Activator.CreateInstance<T>();
            context?.BeforeNavigate(vm);
            await Navigate(vm, context?.RouteName);
        }

        public async Task Navigate(Type type, NavigationContext context = null) {
            var resolutionKey = context?.ResolutionKey;
            var vm = Activator.CreateInstance(type);
            await Navigate(vm, context?.RouteName);
        }

        public Task Navigate(string routeName) {
            if (Route == routeName)
                return Task.FromResult(0);

            var ctx = CreateRouteContext(routeName);
            Route = routeName;
            return ctx.Task;
        }
    }
}