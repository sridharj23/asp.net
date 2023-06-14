<script lang="ts">

  export default {
    emits: ['selectionChanged'],
    data () {
      return {
        selectedIndex: 0, 
        headers: [] as string[],
        tabs: [] as any
      }
    },
    methods: {
      setSelectedTab(tabIndex : number) {
        this.selectedIndex = tabIndex;
        this.$emit('selectionChanged', this.tabs[tabIndex]);
      }
    },
    mounted() {
      console.log(this.tabs);
      this.$slots.default().forEach((item,index) => {
        this.headers.push(item.props?.title?? 'Tab');
        this.tabs.push(item.props?.key?? 'Tab' + index);
      });
      this.setSelectedTab(0);
    }
  }
</script>

<template>
  <div class="container" id="tabContainer">
    <header id="tabHeaderSection">
      <ul>
        <li v-for="(tab, index) in headers" :key="index" :class="selectedIndex == index ? 'tabHead active' : 'tabHead'" @click="setSelectedTab(index)">{{ tab }}</li>
        <li class="tabHead spacer" />
      </ul>
    </header>
    <slot></slot>
  </div>
</template>

<style scoped>
  #tabHeaderSection {
    margin-bottom: 10px;
  }
  #tabHeaderSection ul {
    margin-top: 0;
    margin-bottom: 2px;
    padding: 0;
    display: flex;
    flex-direction: row;
  }
  #tabHeaderSection ul li {
    list-style: none;
    position: relative;
    cursor: pointer;
  }
  #tabContainer {
    height: 100%;
    width: 100%;
  }
</style>