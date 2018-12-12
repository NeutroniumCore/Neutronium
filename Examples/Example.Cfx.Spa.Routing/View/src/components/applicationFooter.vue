<template>
    <v-footer :fixed="fixed" app>
        <span>&copy; {{year}}</span>
    </v-footer>
</template>

<script>
const props = {
  fixed: {
    type: Boolean,
    default: false
  },
  year: {
    type: Number,
    required: false
  }
};

export default {
  props
};
</script>

<style>
</style>
