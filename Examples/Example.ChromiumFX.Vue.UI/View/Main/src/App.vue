<template>
<div id="app" class="fluid container">
  <div id="main-menu" class="jumbotron logo">
    <img src="./assets/logo.png">
    <p>Neutronium Demo Application</p>
  </div>  

    <form>
      <div class="form-group">
        <label for="name">Name</label>
        <input class="form-control" id="name" placeholder="Name" v-model="Name">
      </div>
      <div class="form-group">
        <label for="lastname">Last name</label>
        <input class="form-control" id="lastname" placeholder="Last Name" v-model="LastName">
      </div>
      <div class="form-group">
        <label for="city">City</label>
        <input class="form-control" id="city" placeholder="City" v-model="Local.City">
      </div>
      <div class="form-group">
        <label for="age">Age: {{Age}} years</label>
        <input type="range" id="age" v-model.number="Age">
      </div>
      <div class="form-group" >
        <label for="state">State: {{PersonalState.displayName}}</label>
        <div id="state" class="checkbox" >
          <label v-for="state in States">
              <input type="radio" v-model="PersonalState" :value="state" />
              <span>{{state.displayName}}</span>
          </label>
        </div>
      </div>
      <div class="form-group" >
        <label for="sex">State: {{Sex.displayName}}</label>
        <div id="sex" class="checkbox" >
          <label v-for="sex in Sexes">
              <input type="radio" v-model="Sex" :value="sex" />
              <i class="fa fa-3x" :class="getIcon(sex)" aria-hidden="true"></i>
          </label>
        </div>
      </div>
      <div>
        {{Count}}
      </div>

      <div class="form-group" >
        <label for="skills">Skills</label>
        <ul class="list-group">
          <li class="list-group-item" v-for="element,index in Skills" :key="index"> 
            <div class="row">
              <div class="col-md-5">
                <label class="col-md-2" :for="'element-name-'+index">Name</label>
                <input class="col-md-3 form-control" :id="'element-name-'+index" placeholder="Name" v-model="element.Name">
              </div>
              <div class="col-md-5">
                <label class="col-md-2" :for="'element-type-'+index">Type</label>
                <input class="col-md-3 form-control" :id="'element-type-'+index" placeholder="Type" v-model="element.Type">
              </div>
              <div class="col-md-2 panel-body">
                <command-button :command="RemoveSkill" :arg="element" name="Remove Skill"></command-button>
              </div>
            </div>
          </li>
        </ul>
      </div>

      <command-button :command="Command" name="Add Skill"></command-button>
  </form>
</div>
</template>

<script>
import 'bootstrap/dist/css/bootstrap.css';
import 'font-awesome/less/font-awesome.less';
import commandButton from './components/command-button'

const props={
    viewModel: Object,
    __window__: Object
};

export default {
  name: 'app',
  props,
  components:{
    commandButton
  },
  data () {
    return this.viewModel
  },
  methods:{
    getIcon(sex) {
      switch (sex.name){
         case 'Male':
          return 'fa-male';

        case 'Female':
          return 'fa-female';
      }
      return '';    
    }
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
