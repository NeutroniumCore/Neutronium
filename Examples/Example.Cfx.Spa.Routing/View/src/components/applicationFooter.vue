<template>
    <v-footer :fixed="fixed" app>
        <span class="footer-text">&copy; {{year}}</span>
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
.footer-text {
  margin-left: 10px;
}
</style>
