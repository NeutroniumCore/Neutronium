<template>
    <v-btn small icon @click.stop="execute">
        <v-icon>{{icon}}</v-icon>
    </v-btn>
</template>

<script>
import mixin from "neutronium-vue-simple-command-mixin";

const props = {
  icon: {
    type: String,
    required: true
  }
};

export default {
  mixins: [mixin],
  props
};
</script>

<style>
</style>
