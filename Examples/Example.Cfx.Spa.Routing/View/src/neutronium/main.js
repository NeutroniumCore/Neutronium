import Vue from "vue";
import App from "@/App.vue";
import rawVm from "../../data/vm";
import { install, vueInstanceOption } from "@/install";
import { createVM } from "neutronium-vm-loader";

const vm = updateVM(rawVm);

function updateVM(raw) {
  const vm = createVM(raw);
  vm.ViewModel.Router = { BeforeResolveCommand: null };
  return vm;
}

install(Vue);

var options = vueInstanceOption();
const { router } = options;
/*eslint no-unused-vars: ["error", { "args": "none" }]*/
router.beforeEach((to, _, next) => {
  const name = to.name;
  if (name == null) {
    next();
    return;
  }
  // || "main";
  import(`../../data/${name}/vm.cjson`)
    .then(module => {
      const newVm = updateVM(module.default);
      router.app.ViewModel.CurrentViewModel = newVm.ViewModel.CurrentViewModel;
      next();
    })
    .catch(error => {
      console.log(error);
      console.log(
        `Problem loading file: "../../data/${name}/vm.cjson". Please create corresponding file to be able to . ViewModel will be set to null.`
      );
      router.app.ViewModel.CurrentViewModel = null;
      next();
    });
});

const vueRootInstanceOption = Object.assign({}, vueInstanceOption() || {}, {
  render: h =>
    h(App, {
      props: {
        viewModel: vm.ViewModel,
        __window__: vm.Window
      }
    }),
  data: vm
});
new Vue(vueRootInstanceOption).$mount("#main");
