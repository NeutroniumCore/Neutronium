import Vue from 'vue'
import App from './App.vue'
import rawVm from '../data/vm'
import CircularJson from 'circular-json'

const vm = CircularJson.parse(rawVm);

new Vue({
	components:{
		App
	},
  el: '#main',
  data: vm
})
