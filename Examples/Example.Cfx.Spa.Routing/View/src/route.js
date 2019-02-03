import Router from "vue-router";
import routeDefinitions from "./routeDefinitions";
import { toPromise } from "neutronium-vue-resultcommand-topromise";

function route({ name, children, component, redirect }) {
  return {
    exact: true,
    path: `/${name}`,
    name,
    children,
    component,
    redirect
  };
}

const routes = routeDefinitions.map(route);

const router = new Router({
  mode: 'hash',
  scrollBehavior: () => ({ y: 0 }),
  routes
});

const menu = routeDefinitions.filter(r => r.menu).map(({ name, menu }) => ({
  title: `Resource.Menu_${name}`,
  to: { name },
  icon: menu.icon
}));

function getRouterViewModel(router) {
  const app = router.app;
  if (!app) {
    return null;
  }

  const viewModel = app.ViewModel;
  if (!viewModel) {
    return null;
  }

  return viewModel.Router;
}

function preprocessPath(path) {
  return path.substring(1);
}

/*eslint no-unused-vars: ["error", { "args": "none" }]*/
router.beforeEach(async (to, from, next) => {
  const routerViewModel = getRouterViewModel(router);
  if (!routerViewModel) {
    next();
    return;
  }

  const navigator = routerViewModel.BeforeResolveCommand;
  if (!navigator) {
    next();
    return;
  }

  const destination = preprocessPath(to.path);

  try {
    const navigationResult = await toPromise(navigator, destination);
    if (!navigationResult.Continue) {
      next(false);
    } else if (navigationResult.Redirect) {
      next({ name: navigationResult.Redirect });
    } else {
      router.app.ViewModel.CurrentViewModel = navigationResult.To;
      next();
    }
  }
  catch (error) {
    next(error);
  }
});

/*eslint no-unused-vars: ["error", { "args": "none" }]*/
router.afterEach((to, from, next) => {
  const routerViewModel = getRouterViewModel(router);
  if (!routerViewModel) {
    return;
  }

  const navigator = routerViewModel.AfterResolveCommand;
  if (!navigator) {
    return;
  }
  const destination = preprocessPath(to.path);
  navigator.Execute(destination);
});

export { router, menu };
