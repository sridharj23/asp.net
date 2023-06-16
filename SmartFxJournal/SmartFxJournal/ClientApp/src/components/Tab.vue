<script lang="ts">
  export default {
    props: {
      title: {
        type: String,
        default: 'Tab'
      },
      isActive: {
        type: Boolean,
        default: false
      }
    }
  }
</script>

<template>
  <div id="tabPage" v-show="isActive">
    <slot></slot>
  </div>
</template>

<style scoped>
  #tabPage {
    height: 100%;
    width: 100%;
  }
  .shown {
    display: block;
  }
  .hidden {
    display: none;
  }
</style>