
<script setup>
  import { ref, onMounted, reactive } from 'vue';
  const props = defineProps([ 'customClass' ]);
  let tabContainer = ref(null);
  let tabHeaders = ref(null);
  let tabs = ref(null);
  let activeTabIndex = ref(0);
  
  onMounted(() => {
    tabs.value = [ ...tabContainer.value.querySelectorAll('.tab') ];
		for(let x of tabs.value) {
	    if(x.classList.contains('active')) {
				activeTabIndex = tabs.value.indexOf(x);
			}
    }
  })
  const changeTab = (index) => {
    activeTabIndex = index;
    for(let x of [...tabs.value, ...tabHeaders.value]) {
   		x.classList.remove('active')
    }
		tabs.value[activeTabIndex].classList.add('active')  
		tabHeaders.value[activeTabIndex].classList.add('active')  
  }
</script>

<template>
  <div id="tabs-container" :class="customClass" ref="tabContainer">
    <div id="tab-headers" class="card">
      <ul>
        <!-- this shows all of the titles --> 
        <li v-for="(tab, index) in tabs" :key="index" :class="activeTabIndex == index ? 'chead active' : 'chead'" @click="changeTab(index)" ref="tabHeaders">{{ tab.title }}</li>
      </ul>
    </div>
    <!-- this is where the tabs go, in this slot -->
    <div id="active-tab">
    	<slot></slot>
    </div>
  </div>
</template>

<style>
  #tab-headers ul {
    margin: 0;
    padding: 0;
    display: flex;
  }
  #tab-headers ul li {
    list-style: none;
    position: relative;
    cursor: pointer;
  }
  #active-tab, #tab-headers {
    width: 100%;
  }
  #active-tab {
    height: 94%;
  }
</style>