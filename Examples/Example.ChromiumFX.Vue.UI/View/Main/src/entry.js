import Vue from 'vue'
import App from './App.vue'
import {install} from './install'

install(Vue)
Vue.component('app', App)