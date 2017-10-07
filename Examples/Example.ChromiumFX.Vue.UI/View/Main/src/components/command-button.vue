<template>
  <button type="button" class="btn btn-default" @click="execute" :disabled="disabled">{{name}}</button>
</template>

<script>
import comandMixin from 'neutronium-vue-command-mixin'

export default {
  mixins:[comandMixin],
  props:{
    name: String
  },
    computed:{
        disabled(){
            return !this.canExecute;
        }
    }
}
</script>

<style>
</style>
