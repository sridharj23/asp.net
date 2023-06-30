<script lang="ts">

  export default {
    name: "Card",
    computed: {
      hasFooter() {
        return !!this.$slots.footerSlot;
      },
      hasHeader() {
        return !!this.$slots.headerSlot;
      }
    }
  }

</script>

<template>
  <div id="cardBase" class="container card">
    <header v-show="hasHeader">
      <slot name="headerSlot"/>
    </header>
    <main class="mainSlot">
      <slot/>
    </main>
    <footer v-show="hasFooter">
      <slot name="footerSlot"/>
    </footer>
  </div>
</template>

<style>
  header {
    max-height: 50px;
  }
  main {
    margin: 4px;
    flex-grow: 1;
  }
  footer {
    border-top: 2px solid #ccc;
    color: #666;
    font-size: 0.8em;
    max-height: 50px
  }
</style>