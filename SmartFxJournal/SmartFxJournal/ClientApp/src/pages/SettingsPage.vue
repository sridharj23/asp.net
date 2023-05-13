<script lang="ts">
  import AccountsView from '@/views/AccountsView.vue'
  import CTraderLogin from '@/views/CTraderLogin.vue';

  export default {
    components : {
      AccountsView,
      CTraderLogin
    },
    data() {
      return {
        selectedView: "accounts"
      }
    }
  }
</script>

<template>
    <div class="container" id="mainContainer">
      <header id="sectionHeader">
        <ul>
          <li :class="selectedView == 'accounts' ? 'tabHead active' : 'tabHead'" @click="selectedView = 'accounts'">Registered Accounts</li>
          <li :class="selectedView == 'cTrader' ? 'tabHead active' : 'tabHead'" @click="selectedView = 'cTrader'">CTrader Import</li>
          <li class="tabHead spacer" />
        </ul>
      </header>
      <AccountsView :class="selectedView == 'accounts' ? 'shown' : 'hidden'" id="accountsView"/>
      <CTraderLogin :class="selectedView == 'cTrader' ? 'shown' : 'hidden'" id="cTraderView"/>
    </div>
</template>

<style>
  #mainContainer {
    flex-grow: 1;
    background-color: aliceblue;
  }
  #accountsView {
    flex-grow: 1;
  }
  #cTraderView {
    flex-grow: 1;
  }
  #sectionHeader {
    margin-bottom: 10px;
  }
  #sectionHeader ul {
    margin-bottom: 2px;
    padding: 0;
    display: flex;
    flex-direction: row;
  }
  #sectionHeader ul li {
    list-style: none;
    position: relative;
    cursor: pointer;
  }
  .shown {
    display: block;
  }
  .hidden {
    display: none;
  }
</style>