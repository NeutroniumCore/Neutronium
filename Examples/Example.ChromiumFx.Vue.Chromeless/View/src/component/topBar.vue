<template>
    <b-navbar toggleable="md" type="dark" variant="dark" id="top-menu">

        <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>

        <b-navbar-brand href="#"><img src="../assets/logo.png" height="50px"></b-navbar-brand>

        <b-navbar-brand href="#">Neutronium Chromeless Demo</b-navbar-brand>

        <b-collapse is-nav id="nav_collapse">

            <!-- Right aligned nav items -->
            <b-navbar-nav class="ml-auto">

                <b-nav-form>
                  <b-form-input size="sm" class="mr-sm-2" type="text" placeholder="search" />
                  <b-button size="sm" class="my-2 my-sm-0">Send</b-button>
                </b-nav-form>

                <b-nav-form>
                    <b-button-group>
                        <icon-button icon="window-minimize" :command="window.Minimize"> </icon-button>
                        <icon-button :icon="middleIcon" :command="window.Normalize"> </icon-button>
                        <icon-button icon="times" :command="window.Close"> </icon-button>
                    </b-button-group>
                </b-nav-form>

            </b-navbar-nav>

        </b-collapse>
    </b-navbar>
</template>
<script>
import iconButton from "./iconButton";
const props = {
  window: {
    type: Object,
    required: true
  }
};

export default {
  components: {
    iconButton
  },
  props,
  computed: {
    middleIcon() {
      return this.window.State.displayName == "Normal"
        ? "window-maximize"
        : "window-restore";
    }
  }
};
</script>
<style>
#top-menu {
  -webkit-app-region: drag;
}

#top-menu form {
  -webkit-app-region: no-drag;
}

#top-menu button {
  -webkit-app-region: no-drag;
}
</style>
