import Vue from 'vue'
import App from './App.vue'
import {install, vueInstanceOption} from './install'
import vueHelper from 'vueHelper'

install(Vue)
vueHelper.setOption(vueInstanceOption())
Vue.component('app', App)