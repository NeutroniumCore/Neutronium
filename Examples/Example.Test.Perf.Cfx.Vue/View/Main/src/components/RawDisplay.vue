<template>
    <pre class="raw">{{object}}</pre>
</template>

<script>
export default {
  props: {
    object: String
  }
};
</script>

<style>
pre.raw {
  height: 400px;
  overflow-y: scroll;
  background-color: #efefef;
}
</style>
