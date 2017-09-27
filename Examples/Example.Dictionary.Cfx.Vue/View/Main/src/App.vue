<template>
    <div id="app" class="fluid container">
        <div id="main-menu" class="jumbotron logo">
            <img src="./assets/logo.png">
            <p>Neutronium Dictionnary Mapping</p>
        </div>

        <div class="list-group col-md-3">
            <raw-display :object="Dynamic">
            </raw-display>
        </div>

        <command-button :command="ChangeAttribute" name="Change attribute" class="primary"></command-button>

        <command-button :command="AddAttribute" name="Add attribute" class="secondary"></command-button>

    </div>
</template>

<script>
import 'bootstrap/dist/css/bootstrap.css';
import 'font-awesome/less/font-awesome.less';
import rawDisplay from './components/RawDisplay'
import commandButton from './components/command-button'

const props={
    viewModel: Object
};

export default {
    name: 'app',
    props,
    components:{
        rawDisplay,
        commandButton
    },
    data () {
        return this.viewModel
    }
}
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
