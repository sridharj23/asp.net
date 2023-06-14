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
    border: 2px solid purple;
  }
  .shown {
    display: block;
  }
  .hidden {
    display: none;
  }
</style>