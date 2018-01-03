import Vue from 'vue'
import App from './App.vue'
import rawVm from '../data/vm'
import { createVM } from 'neutronium-vm-loader'
import { install, vueInstanceOption } from './install'

const vm = createVM(rawVm);

const vueRootInstanceOption = Object.assign({}, vueInstanceOption || {}, {
    components: {
        App
    },
    data: vm
});

install(Vue)
new Vue(vueRootInstanceOption).$mount('#main')
