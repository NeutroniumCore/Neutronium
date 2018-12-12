import Vue from "vue";
import App from "./App.vue";
import { install, vueInstanceOption } from "./install";
import vueHelper from "vueHelper";

function buildVueOption(vm) {
  var option = vueInstanceOption(vm);
  option.render = function(h) {
    const prop = {
      props: {
        viewModel: this.$data.ViewModel,
        __window__: this.$data.Window
      }
    };
    return h(App, prop);
  };
  return option;
}

install(Vue);
vueHelper.setOption(buildVueOption);
