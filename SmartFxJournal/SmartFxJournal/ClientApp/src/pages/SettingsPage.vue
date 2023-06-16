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
        <Tab class="tabPage" :title="'Registered Accounts'" :key="'accounts'" :isActive="selectedTab == 'accounts'">
          <AccountsView class="tabContent" id="accountsView"/>
        </Tab>
        <Tab :title="'CTrader Import'" :key="'ctrader'" :isActive="selectedTab == 'ctrader'">
          <CTraderLogin class="tabContent" id="cTraderView"/>
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
  .tabPage {
    height: 100%;
  }
  .tabContent {
    height: 100%;
  }
</style>