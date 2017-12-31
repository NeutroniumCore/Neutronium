import BootstrapVue from 'bootstrap-vue'
import Icon from 'vue-awesome/components/Icon'

function install(Vue) {
    Vue.use(BootstrapVue);
    Vue.component('icon', Icon);
}

function vueInstanceOption() {
    //Return vue global option here, such as vue-router, vue-i18n, mix-ins, .... 
    return {}
}

export {
    install,
    vueInstanceOption
} 