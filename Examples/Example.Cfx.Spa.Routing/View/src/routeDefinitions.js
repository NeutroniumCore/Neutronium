const routeDefinitions = [
  {
    name: "",
    redirect: { name: "main" }
  },
  {
    name: "main",
    component: () => import("./pages/main.vue"),
    menu: { icon: "fa-television" }
  },
  {
    name: "about",
    component: () => import("./pages/about.vue"),
    menu: { icon: "info" }
  }
];

export default routeDefinitions;
