import VueI18n from "vue-i18n";
import messages from "./message";

/*eslint no-unused-vars: ["error", { "args": "none" }]*/
function install(Vue) {
  //Call vue use here if needed

  Vue.use(VueI18n);
}

/*eslint no-unused-vars: ["error", { "args": "none" }]*/
function vueInstanceOption(vm) {
  const i18n = new VueI18n({
    locale: "en-US", // set locale
    messages // set locale messages
  });
  //Return vue global option here, such as vue-router, vue-i18n, mix-ins, ....
  return {
    i18n
  };
}

export { install, vueInstanceOption };
