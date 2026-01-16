<template>
  <div :class="{'dark': isDarkMode}">
    <n-config-provider :theme="naiveTheme" :locale="zhCN" :date-locale="dateZhCN">
      <n-dialog-provider>
        <n-message-provider>
          <router-view />
        </n-message-provider>
      </n-dialog-provider>
    </n-config-provider>
  </div>
</template>

<script setup lang="ts">
import { NMessageProvider, NDialogProvider, NConfigProvider, dateZhCN, zhCN } from 'naive-ui'
import { useThemeStore } from './stores/ThemeStore'
import { storeToRefs } from 'pinia'

const store = useThemeStore()
const { naiveTheme, isDarkMode } = storeToRefs(store)
</script>

<style>
/* 全局样式重置 */
* {
  box-sizing: border-box;
}

html,
body {
  margin: 0;
  padding: 0;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

/* 平滑滚动 */
html {
  scroll-behavior: smooth;
}

/* 统一颜色变量 */
:root {
  --color-background: #ffffff;
  --color-text: #000000;
  --color-border: #e5e7eb;
  --color-hover: #f3f4f6;
}

.dark {
  --color-background: #111827;
  --color-text: #f9fafb;
  --color-border: #374151;
  --color-hover: #1f2937;
}
</style>