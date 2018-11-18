<template>
  <nav
    class="side-menu"
    role="navigation"
  >
    <div class="group">
      <ul class="menu-list">
        <li>
          <router-link
            to="/"
            active-class="is-active"
            exact
          >
            <span class="icon">
              <font-awesome-icon icon="calendar-alt" />
            </span>
            <span>Today</span>
          </router-link>
        </li>
        <li>
          <router-link
            to="/collections"
            active-class="is-active"
            exact
          >
            <span class="icon">
              <font-awesome-icon icon="clone" />
            </span>
            <span>Collections</span>
          </router-link>
          <ul>
            <li>
              <router-link
                to="/collections/development-1"
                active-class="is-active"
              >
                Development
              </router-link>
            </li>
            <li>
              <router-link
                to="/collections/photography-2"
                active-class="is-active"
              >
                Photography
              </router-link>
            </li>
            <li>
              <router-link
                to="/collections/music-2"
                active-class="is-active"
              >
                Music
              </router-link>
            </li>
            <li>
              <button class="button">
                <span class="icon is-small">
                  <font-awesome-icon icon="plus-circle" />
                </span>
                <span>New collection</span>
              </button>
            </li>
          </ul>
        </li>
      </ul>
    </div>
  </nav>
</template>

<script>
export default {
  name: 'AsideMenu',
};
</script>

<style lang="scss" scoped>
  .side-menu {
    min-height: 100%;
    min-height: 100vh;
    background-color: #161c27;
    color: #a7a9af;
    padding-top: .25rem;

    a {
      color: inherit;
      transition: background-color .2s ease-in-out, color .2s ease-in-out;

      &:hover,
      &.is-hover,
      &:active,
      &.is-active,
      &:focus {
        color: #f5f5f5;
      }
    }

    .group {
      padding-left: .25rem;
      margin-top: .8rem;

      &:first-child {
        margin-top: 0;
      }
    }

    .button {
      display: inline-block;
      width: 100%;
      text-align: left;
      background: none;
      border: none;
      color: #ffba49;
      padding-top: .25rem;
      padding-bottom: .25rem;
      height: inherit;
    }
  }

  .menu-label {
    font-size: .75em;
    letter-spacing: .1em;
    text-transform: uppercase;

    &:not(:last-child) {
      margin-bottom: 1em;
    }
  }

  .menu-list {
    list-style: none;
    line-height: 1.25;

    ul {
      margin-left: 1.3rem;
    }

    a {
      border-radius: 2px 0 0 2px;
      display: block;
      padding: .25em 1.5em;

      &.is-active {
        background-color: #20a39e;
      }
    }

    .button {
      padding-left: .25rem;
    }
  }
</style>
