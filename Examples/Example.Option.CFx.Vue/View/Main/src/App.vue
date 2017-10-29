<template>
  <div>
    <img src="./assets/logo.png">
    <h1>{{ $t("Resource.Welcome") }}</h1>
    <h2>{{ $t("Resource.MyFriend") }}</h2>
    <h2>Related Links</h2>
    <ul>
      <li><a target="_blank" href="https://vuejs.org">Core Vue Docs</a></li>
      <li><a target="_blank" href="https://forum.vuejs.org">Vue Forum</a></li>
      <li><a target="_blank" href="https://github.com/David-Desmaisons/Neutronium">Neutronium</a></li>
      <li><a target="_blank" href="https://github.com/David-Desmaisons/neutronium-vue">Neutronium Vue template</a></li>
      <li><a target="_blank" href="https://github.com/David-Desmaisons/NeutoniumDemo">Neutronium Vue application demo</a></li>
    </ul>
  </div>
</template>

<script>
const props={
  viewModel: Object,
  __window__: Object
};

export default {
  name: 'app',
  props,
  data () {
    return this.viewModel
  }
}
</script>

<style>
#main {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}

img {
  height: 300px;
}

h1, h2 {
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
</style>
