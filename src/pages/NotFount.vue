<script setup lang="ts">

import {LiquidGlass} from "@wxperia/liquid-glass-vue";
const handleClick = () => {
  console.log('按钮被点击！')
}
</script>

<template>
  <div class="home-bg">
    <LiquidGlass
        :displacement-scale="64"
        :blur-amount="0.1"
        :saturation="130"
        :aberration-intensity="2"
        :elasticity="0.35"
        :corner-radius="100"
        padding="8px 16px"
        @click="handleClick"
    >
      <span class="text-white font-medium">点击我</span>
    </LiquidGlass>
  </div>
</template>

<style scoped>
.home-bg {
  background-image: linear-gradient(to bottom right, #ff6b6b, #7171ae, #10bce7);
  min-height: 100vh;
  height: 100%;
  left: 0;
  -o-object-fit: cover;
  object-fit: cover;
  position: fixed;
  top: 0;
  transition: filter .25s, opacity 1s, transform .25s;
  width: 100%;
  z-index: -9999;
}

@media (prefers-color-scheme: dark) {
  .home-bg {
    background-image: linear-gradient(to top, #1e3c72 0%, #1e3c72 1%, #2a5298 100%);
  }
}
</style>