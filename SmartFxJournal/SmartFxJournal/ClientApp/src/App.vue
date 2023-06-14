<script setup>
  import { RouterLink, RouterView } from 'vue-router';
  import StatusBar from '@/components/StatusBar.vue'
  import Card from '@/components/Card.vue';
</script>

<template>
  <div id="mainFrame">
    <Card>
      <div class="wrapper">
        <nav>
          <RouterLink to="/">Summary</RouterLink>
          <RouterLink to="/history">History</RouterLink>
          <RouterLink to="/notes">Notes</RouterLink>
          <RouterLink to="/reconcile">Reconcile</RouterLink>
        </nav>
      </div>
    </Card>
    <RouterView class="routerView"/>
    <Card id="StatusBar">
      <StatusBar id="statusdisplay"/>
    </Card>
  </div>
</template>

<style scoped>

  #mainFrame {
    display: flex;
    flex-direction: column;
  }
  .routerView {
    flex-grow: 1;
    margin-top: 5px;
    height: 90vh;
  }
  nav {
    width: 100%;
    font-size: 23px;
    text-align: left;
    padding-bottom: 0.5rem;
  }
  nav a.router-link {
    color: darkgray;
    font-weight: bold;
  }
  nav a.router-link-exact-active {
    color: dodgerblue;
    font-weight: bold;

  }
  nav a {
    display: inline-block;
    padding: 0 1rem;
    margin-top: 10px;
    border-left: 1px solid var(grey);
  }
  #StatusBar {
    margin-top: 8px;
    height: 35px;
    bottom: 5px;
  }
  #statusdisplay {
    margin-top: 4px;
    margin-left: 5px;
  }

</style>