<template>
    <div id="app" class="fluid container">
        <div id="main-menu" class="jumbotron logo">
            <img src="./assets/logo.png">
            <p>Neutronium ExpandoObject and DynamicObject mapping</p>
        </div>
        <div class="row">
          <div class="list-group col-md-6">
              <h3>ExpandoObject</h3>
              <raw-display :object="ExpandoObject">
              </raw-display>

            <div>
              <command-button :command="ChangeAttribute" name="Change attribute" class="btn-primary"></command-button>
              <command-button :command="AddAttribute" name="Add attribute" class="btn-secondary"></command-button>
            </div>

          </div>

          <div class="list-group col-md-6">
              <h3>DynamicObject</h3>
              <raw-display :object="DynamicObject">
              </raw-display>
          </div>
        </div>
    </div>
</template>

<script>
import rawDisplay from "./components/RawDisplay";
import commandButton from "./components/command-button";

const props = {
  viewModel: Object
};

export default {
  name: "app",
  props,
  components: {
    rawDisplay,
    commandButton
  },
  data() {
    return this.viewModel;
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
