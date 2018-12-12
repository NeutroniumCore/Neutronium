<template>
    <v-btn :color="color" :disabled="!canExecute"  @click.stop="execute">
        {{text}}
    </v-btn>
</template>

<script>
import mixin from "neutronium-vue-simple-command-mixin";

const props = {
  text: {
    type: String,
    required: true
  },
  color: {
    type: String,
    default: "primary"
  }
};

export default {
  mixins: [mixin],
  props
};
</script>

<style>
</style>
