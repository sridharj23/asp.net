<template>
  <div class="container card">
    <header>
      <slot name="header"></slot>
    </header>
    <main class="mainSlot">
      <slot></slot>
    </main>
    <footer>
      <slot name="footer"></slot>
    </footer>
  </div>
</template>

<style>
  main {
    margin: 4px;
    flex-grow: 1;
  }
  footer {
    border-top: 2px solid #ccc;
    color: #666;
    font-size: 0.8em;
  }
</style>