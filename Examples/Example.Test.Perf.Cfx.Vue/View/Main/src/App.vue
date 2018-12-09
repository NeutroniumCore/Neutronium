<template>
  <div
    id="app"
    class="fluid container"
  >
    <div
      id="main-menu"
      class="jumbotron logo"
    >
      <img src="./assets/logo.png">
      <p>Neutronium Demo Perf Application</p>
    </div>

    <div class="row">
      <div class="col-md-9">
        
        <raw-display :object="bigVm">
        </raw-display>
        
         <command-button :command="BuildBigModel" name="Build Big Model">
         </command-button>

      </div>

      <div class="col-md-3">
        <counter
          :counter="Counter"
          :state="State"
          :command="Count"
        >
        </counter>
      </div>
    </div>

  </div>
</template>

<script>
import "bootstrap/dist/css/bootstrap.css";
import "font-awesome/less/font-awesome.less";
import commandButton from "./components/command-button";
import counter from "./components/counter";
import rawDisplay from "./components/RawDisplay";

const props = {
  viewModel: Object,
  __window__: Object
};

export default {
  name: "app",
  props,
  components: {
    commandButton,
    counter,
    rawDisplay
  },
  data() {
    return this.viewModel;
  },
  computed: {
    bigVm() {
      return JSON.stringify(this.viewModel.Big, null, 2);
    }
  }
};
</script>

<style>
#app .logo {
  text-align: center;
}

#app .logo img {
  width: 100px;
}

.panel-body {
  position: relative;
  min-height: 200px;
  height: 100%;
}
</style>
