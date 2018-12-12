<template>
  <div class="application-notifications">
    <notifications group="app" :reverse="true"  position="top center" :width="700">
      <template slot="body" slot-scope="props">

        <v-alert :color="props.item.type" :icon="icon(props.item.type)" :value="true" dismissible @input="props.close()">
          <h6 class='black--text'>{{props.item.title}}</h6>
          <p>{{props.item.text}}</p>
        </v-alert>
    
      </template>
    </notifications>
  </div>
</template>

<script>
const icons = {
  error: "warning",
  warning: "priority_high",
  info: "info",
  success: "check_circle"
};

export default {
  methods: {
    showMessage(message) {
      const notification = {
        title: message.Title,
        type: message.Type,
        text: message.Content,
        duration: message.duration || message.Type === "error" ? -1 : 4000,
        group: "app"
      };
      this.$notify(notification);
      this.$emit("notified", notification);
    },
    icon(type) {
      return icons[type] || icons.information;
    }
  }
};
</script>

<style>
.application-notifications p {
  overflow-wrap: break-word;
}

.application-notifications div {
  overflow-y: auto;
}
</style>
