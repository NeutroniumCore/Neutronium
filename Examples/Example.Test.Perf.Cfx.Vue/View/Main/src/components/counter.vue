<template>
    <div>
        <div>
            {{counter}}
        </div>

        <div>
            {{state}}
        </div>
        <command-button :command="command" name="Run"></command-button>
    </div>
</template>

<script>
import commandButton from "./command-button";

export default {
  props: {
    counter: Number,
    state: String,
    command: Object
  },
  components: {
    commandButton
  }
};
</script>

<style>
</style>
