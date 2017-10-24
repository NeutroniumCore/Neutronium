import Vue from 'vue'
import App from './App.vue'
import rawVm from '../data/vm'
import CircularJson from 'circular-json'
import {install, vueInstanceOption} from './install'

function updateVm(vm) {
    var window = vm.__window__
    if (window) {
        delete vm.__window__
        return { ViewModel: vm, Window: window }
    }
    return vm;
}

const vm = updateVm(CircularJson.parse(rawVm));

install(Vue)
const vueRootInstanceOption = Object.assign({}, vueInstanceOption() || {}, {
    components: {
        App
    },
    data: vm
});
new Vue(vueRootInstanceOption).$mount('#main')
