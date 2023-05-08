<script setup>
  import TabControl from './components/TabControl.vue';
  import Tab from './components/Tab.vue';
  import SummaryPage from './pages/SummaryPage.vue';
</script>

<template>
    <div id="MainPage" style="text-align:center">
      <TabControl id="MainTab">
        <Tab active="true" title="Summary">
          <SummaryPage id="summaryPage"/>
        </Tab>
        <Tab title="Trades">
          
        </Tab>
        <Tab title="Analysis">
          Morbi posuere, mauris eu vehicula tempor, nibh orci consectetur tortor, id eleifend dolor sapien ut augue.
        </Tab>
      </TabControl>
      <div id="StatusBar"/>
    </div>
</template>

<style scoped>
  #MainPage {
    display: flex;
    flex-direction: column;
    position: fixed;
    left: 0;
    top: 0;
    width: 98.5vw;
    height: 99%;
    margin: 5px;
  }

  #MainTab {
    flex-grow: 1;
  }

  #StatusBar {
    left: 0;
    bottom: 0;
    margin: 5px;
    width: 99.5%;
    height: 35px;
    border: 1px solid darkgray;
    border-radius: 5px;
    box-shadow: 1px 1px 4px 2px silver;
  }

  #summaryPage {
    display: block;
    margin: 5px;
    width: 99.5%;
    height: 100%;
  }
</style>