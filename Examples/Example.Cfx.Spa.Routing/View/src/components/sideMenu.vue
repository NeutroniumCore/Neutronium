<template>
  <v-navigation-drawer id="side-menu"
      :mini-variant="mini"
      :clipped="clipped"
      :value="value"
      @input="drawerChanged"
      :marginTop="0"
      :maxHeight="0"
      app
    >

    <v-divider></v-divider>

      <v-list>
        <v-list-tile v-for="(item, i) in items" :key="i" :to="item.to" router ripple>

          <v-list-tile-action>
            <v-icon light v-html="item.icon"></v-icon>
          </v-list-tile-action>

          <v-list-tile-content>
            <v-list-tile-title v-text="$t(item.title)"></v-list-tile-title>
          </v-list-tile-content>

        </v-list-tile>
      </v-list>

    </v-navigation-drawer>
</template>

<script>
const props = {
  items: {
    type: Array,
    required: true
  },
  value: {
    type: Boolean
  },
  clipped: {
    type: Boolean,
    default: true
  }
};

export default {
  props,
  data() {
    return {
      mini: true
    };
  },
  methods: {
    drawerChanged(value) {
      this.$emit("input", value);
    }
  }
};
</script>

<style>
#side-menu button {
  -webkit-app-region: no-drag;
}
</style>
