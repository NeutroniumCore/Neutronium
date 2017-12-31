<template>
    <b-button :class="{'disabled': !canExecute}" variant="link">
        <icon @click.native.prevent="execute" :name="icon" class="icon"></icon>
    </b-button>
</template>
<script>
import comandMixin from "neutronium-vue-command-mixin";

const props = {
  icon: {
    type: String,
    required: true
  }
};

export default {
  props,
  mixins: [comandMixin]
};
</script>
<style>
.icon {
  color: white;
}
</style>
