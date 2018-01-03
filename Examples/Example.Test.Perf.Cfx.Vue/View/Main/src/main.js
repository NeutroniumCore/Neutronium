import Vue from 'vue'
import App from './App.vue'
import rawVm from '../data/vm'
import { createVM } from 'neutronium-vm-loader'

const vm = createVM(rawVm);

new Vue({
	components:{
		App
	},
  el: '#main',
  data: vm
})
