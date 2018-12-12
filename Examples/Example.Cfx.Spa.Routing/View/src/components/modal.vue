<template>
    <v-dialog :value="value" v-if="viewModel" :persistent="!!viewModel.CancelCommand" :max-width="300" @input="valueChanged">
        <v-card class="application-modal">
            <v-card-title class="headline">{{viewModel.Title}}</v-card-title>
            <v-card-text>{{viewModel.Message}}</v-card-text>
            <v-card-actions v-if="viewModel.OkCommand">
                <v-spacer></v-spacer>
                <text-button v-if="viewModel.CancelCommand" :text="viewModel.CancelMessage" color="error" :command="viewModel.CancelCommand" @afterExecute="close()"></text-button>
                <text-button :text="viewModel.OkMessage" color="green darken-1" :command="viewModel.OkCommand" @afterExecute="close()"></text-button>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
import textButton from "../components/textButton";

const props = {
  value: {
    type: Boolean,
    default: true
  },
  viewModel: {
    required: true
  }
};

export default {
  components: {
    textButton
  },
  props,
  methods: {
    close() {
      this.$emit("input", false);
    },
    valueChanged(value) {
      this.$emit("input", value);
    }
  }
};
</script>
<style>
.application-modal div {
  word-wrap: break-word;
}
</style>
