<template>
  <div :class="{ 'dark': isDark }">
    <n-config-provider :theme="theme">
      <n-dialog-provider>
        <n-message-provider>
          <router-view/>
        </n-message-provider>
      </n-dialog-provider>
    </n-config-provider>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref, computed } from 'vue'
import { NMessageProvider, NDialogProvider, NConfigProvider, darkTheme } from 'naive-ui'
import Layout from './components/Layout.vue'

// 从localStorage获取主题偏好，如果没有则跟随系统
const savedTheme = localStorage.getItem('theme')
let isDark = ref(savedTheme === 'dark' || (savedTheme === null && window.matchMedia('(prefers-color-scheme: dark)').matches))
const theme = computed(() => (isDark.value ? darkTheme : null))

// 切换主题
const toggleTheme = () => {
  isDark.value = !isDark.value
  localStorage.setItem('theme', isDark.value ? 'dark' : 'light')
  document.documentElement.classList.toggle('dark', isDark.value)
}

// 系统主题变化监听
let mediaQuery = window.matchMedia('(prefers-color-scheme: dark)')

const handler = (e: MediaQueryListEvent) => {
  // 只有当用户没有显式设置主题偏好时才跟随系统
  if (!localStorage.getItem('theme')) {
    isDark.value = e.matches
  }
}

onMounted(() => {
  // 初始化时设置dark类
  document.documentElement.classList.toggle('dark', isDark.value)
  mediaQuery.addEventListener('change', handler)
})

// 组件卸载时移除监听
onUnmounted(() => {
  mediaQuery.removeEventListener('change', handler)
})
</script>

<style>
/* 全局样式重置 */
* {
  box-sizing: border-box;
}

html, body {
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