<template>
  <div id="app">
    <drawer>
      <template slot="menu">
        <aside-menu />
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
import AsideMenu from '@/components/AsideMenu.vue';

export default {
  name: 'App',
  components: {
    Drawer,
    AsideMenu,
  },
};
</script>

<style lang="scss">
@import './styles/default.scss';
</style>
