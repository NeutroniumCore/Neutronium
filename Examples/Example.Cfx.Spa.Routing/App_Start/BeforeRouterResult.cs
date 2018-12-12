namespace Example.Cfx.Spa.Routing.App_Start {
    public struct BeforeRouterResult {
        private BeforeRouterResult(string redirect, object viewModel) {
            Redirect = redirect;
            Continue = true;
            To = viewModel;
        }

        private BeforeRouterResult(bool continueRoute, object viewModel) {
            Redirect = null;
            Continue = continueRoute;
            To = viewModel;
        }

        public static BeforeRouterResult Cancel() {
            return new BeforeRouterResult(false, null);
        }

        public static BeforeRouterResult Ok(object viewModel) {
            return new BeforeRouterResult(null, viewModel);
        }

        public static BeforeRouterResult CreateRedirect(string routeName) {
            return new BeforeRouterResult(routeName, null);
        }

        public string Redirect { get; }
        public bool Continue { get; }
        public object To { get; }
    }
}
