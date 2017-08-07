<template>
<div id="app" class="fluid container">
  <div id="main-menu" class="jumbotron logo">
    <img src="./assets/logo.png">
    <p>Neutronium Demo Perf Application</p>
  </div>

  <div>
    {{Counter}}
  </div>

  <div>
    {{State}}
  </div>

  <command-button :command="Count" name="Run"></command-button>
</div>
</template>

<script>
import 'bootstrap/dist/css/bootstrap.css';
import 'font-awesome/less/font-awesome.less';
import commandButton from './components/command-button'

const props={
  viewModel: Object,
};

export default {
  name: 'app',
  props,
  components:{
    commandButton
  },
  data () {
    return this.viewModel
  }
}
</script>

<style>
  #app .logo{
  text-align: center;
  }

  #app .logo img{
  width: 100px;
  }

  .panel-body {
  position: relative;
  min-height:200px;
  height:100%;
  }
</style>
