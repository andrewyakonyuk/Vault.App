<template>
  <div
    class="drawer"
    :class="{ 'is-open': isDrawerOpen }"
    @touchstart="onTouchStart"
    @touchend="onTouchEnd"
  >
    <aside class="drawer-menu">
      <hamburger-button @hamburger-click="toggleDrawer" />
      <slot name="menu" />
    </aside>
    <div
      class="drawer-catcher"
      @click="toggleDrawer(false)"
    />
    <section class="drawer-content">
      <hamburger-button @hamburger-click="toggleDrawer" />
      <slot name="content" />
    </section>
  </div>
</template>

<script>
import HamburgerButton from '@/components/HamburgerButton.vue';

export default {
  name: 'Drawer',
  components: {
    HamburgerButton,
  },
  data() {
    return {
      isDrawerOpen: false,
    };
  },
  methods: {
    toggleDrawer(to) {
      this.isDrawerOpen = typeof to === 'boolean' ? to : !this.isDrawerOpen;
    },

    // side swipe
    onTouchStart(e) {
      this.touchStart = {
        x: e.changedTouches[0].clientX,
        y: e.changedTouches[0].clientY,
      };
    },

    onTouchEnd(e) {
      const dx = e.changedTouches[0].clientX - this.touchStart.x;
      const dy = e.changedTouches[0].clientY - this.touchStart.y;
      if (Math.abs(dx) > Math.abs(dy) && Math.abs(dx) > 40) {
        if (dx > 0 && this.touchStart.x <= 80) {
          this.toggleDrawer(true);
        } else {
          this.toggleDrawer(false);
        }
      }
    },
  },
};
</script>

<style lang="scss">
  @import '@/styles/_vars.scss';
  $drawerMenuWidth: 18rem;

  .drawer-menu {
    text-align: left;
    position: fixed;
    width: $drawerMenuWidth;
    background-color: hsl(0, 0%, 96%);
    z-index: 100;
    top: 0;
    border-right: 1px solid #eaecef;
    left: 0;
    bottom: 0;
    overflow-y: auto;
    transform: translateX(-100%);
    transition: transform .2s ease;
    will-change: transform;
    box-shadow: inset -1px 0 0 rgba(0,0,0,.1);
  }

  .drawer-catcher {
    position: fixed;
    z-index: 90;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    display: none;
  }

  .drawer.is-open {
    .drawer-menu {
      transform: translateX(0);
    }

    .drawer-catcher {
      display: block;
    }
  }

  @media(min-width: $tablet) {
    .hamburger-button {
      display: none;
    }

    .drawer-catcher {
      display: none;
    }

    .drawer-menu {
      z-index: inherit;
      padding-top: 0.8rem;
      border-right: none;
      transform: translateX(0);
    }

    .drawer-content {
      padding-left: $drawerMenuWidth;
    }
}
</style>
