<script setup>
  const props = defineProps([ 'active' ]);
</script>

<template>
  <div class="tab container" :class="(active == 'true') ? 'active' : ''" ref="tabs">
    <slot class="tabSlot"></slot>
  </div>
</template>

<style>
  .tab {
    display: none;
  }
  .tab.active {
    display: block;
    height: 100%;
  }
</style>