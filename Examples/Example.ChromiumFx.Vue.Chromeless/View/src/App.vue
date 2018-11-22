<template>
  <div class="main-div">
    <top-bar :window="viewModel.Window"></top-bar>
  </div>
</template>

<script>
import "bootstrap/dist/css/bootstrap.css";
import "bootstrap-vue/dist/bootstrap-vue.css";
import "vue-awesome/icons";
import topBar from "./component/topBar";

const props = {
  viewModel: Object,
  __window__: Object
};

export default {
  name: "app",
  props,
  components: {
    topBar
  },
  data() {
    return this.viewModel;
  }
};
</script>

<style>
#main {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

h1,
h2 {
  font-weight: normal;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
  display: inline-block;
  margin: 0 10px;
}

a {
  color: #42b983;
}

.main-div {
  color: #42b983;
  height: 100%;
  border: black;
  border-style: solid;
}
</style>
