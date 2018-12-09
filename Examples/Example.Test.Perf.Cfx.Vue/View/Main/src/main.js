import Vue from "vue";
import App from "./App.vue";
import rawVm from "../data/vm";
import { install, vueInstanceOption } from "./install";
import { createVM } from "neutronium-vm-loader";

const vm = createVM(rawVm);

install(Vue);

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
