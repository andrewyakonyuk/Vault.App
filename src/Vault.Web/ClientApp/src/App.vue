<template>
  <div id="app">
    <drawer>
      <template slot="menu">
        <nav>
          <router-link to="/">Home</router-link> |
          <router-link to="/about">About</router-link>
        </nav>
      </template>
      <template>
        <template slot="content">
          <router-view />
        </template>
      </template>
    </drawer>
  </div>
</template>

<script>
import Drawer from '@/components/Drawer.vue';

export default {
  name: 'App',
  components: {
    Drawer,
  },
};
</script>

<style lang="scss">
@import './styles/default.scss';
</style>
