<template>
    <div id="app" class="fluid container">
        <div id="main-menu" class="jumbotron logo">
            <img src="./assets/logo.png">
            <p>{{Localization.NeutroniumLocalizationExample}}</p>
        </div>

        <div class="col-md-2">
            <span>{{Localization.Language}}</span>
            <select v-model="Langage">
                <option v-for="lang in Langages" :value="lang" :key="lang">{{lang}}</option>
            </select>
        </div>

        <div class="col-md-10">
            <h1>{{Localization.Hello}}</h1>
            <h2>{{Localization.Welcome}}</h2>
            <h3>{{Localization.MyFriend}}</h3>
        </div>
    </div>
</template>

<script>
import "bootstrap/dist/css/bootstrap.css";
import "font-awesome/less/font-awesome.less";

const props = {
  viewModel: Object
};

export default {
  name: "app",
  props,
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
