<script lang="ts">
  import AccountsView from '@/views/AccountsView.vue'
  import CTraderLogin from '@/views/CTraderLogin.vue';
  import TabControl from '@/components/TabControl.vue';
  import Tab from '@/components/Tab.vue';

  export default {
    components : {
      AccountsView,
      CTraderLogin,
      TabControl,
      Tab
    },
    data() {
      return {
        selectedTab: "accounts"
      }
    },
    methods: {
      handleSelectionChange(tabKey: string) {
        this.selectedTab = tabKey;
      }
    }
  }
</script>

<template>
    <div class="container" id="mainContainer">
      <TabControl id="settingsTab" @selectionChanged="handleSelectionChange">
        <Tab :title="'Registered Accounts'" :key="'accounts'" :isActive="selectedTab == 'accounts'">
          <AccountsView id="accountsView"/>
        </Tab>
        <Tab :title="'CTrader Import'" :key="'ctrader'" :isActive="selectedTab == 'ctrader'">
          <CTraderLogin id="cTraderView"/>
        </Tab>
      </TabControl>
    </div>
</template>

<style>
  #mainContainer {
    flex-grow: 1;
  }
  #accountsView {
    flex-grow: 1;
  }
  #cTraderView {
    flex-grow: 1;
  }
  #settingsTab {
    height: 100%;
  }
</style>