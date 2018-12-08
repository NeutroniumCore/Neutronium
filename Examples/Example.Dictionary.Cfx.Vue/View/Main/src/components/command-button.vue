<template>
  <button type="button" class="btn" @click="execute">{{name}}</button>
</template>

<script>
import comandMixin from "neutronium-vue-command-mixin";

export default {
  mixins: [comandMixin],
  props: {
    name: String
  }
};
</script>

<style>
</style>
