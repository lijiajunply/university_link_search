<template>
  <div class="min-h-screen flex flex-col bg-[--bg-primary] transition-colors duration-300">
    <!-- 顶部导航栏 - 苹果风格毛玻璃效果 -->
    <header
      class="sticky top-0 z-50 backdrop-blur-2xl bg-[--glass-bg] border-b border-[--border-primary] transition-all duration-300">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between h-14 sm:h-16">
          <!-- Logo 区域 -->
          <router-link to="/" class="flex items-center gap-2 sm:gap-3 group">
            <div
              class="w-8 h-8 sm:w-9 sm:h-9 rounded-full bg-linear-to-br from-blue-500 to-blue-600 flex items-center justify-center shadow-lg shadow-blue-500/30 group-hover:shadow-blue-500/50 transition-all duration-300 group-hover:scale-105">
              <Icon icon="solar:home-smile-bold" class="w-4 h-4 sm:w-5 sm:h-5 text-white" />
            </div>
            <span class="text-base sm:text-lg font-semibold text-[--text-primary] tracking-tight hidden xs:block">
              建大导航
            </span>
          </router-link>

          <!-- 桌面端导航 -->
          <nav class="hidden md:flex items-center gap-1">
            <router-link
              v-for="item in navItems"
              :key="item.path"
              :to="item.path"
              class="relative px-3 lg:px-4 py-2 rounded-xl text-sm lg:text-base font-medium transition-all duration-200 group"
              :class="isActiveRoute(item.path) 
                ? 'text-blue-600 dark:text-blue-400' 
                : 'text-[--text-secondary] hover:text-[--text-primary] hover:bg-[--hover-bg]'">
              <div class="flex items-center gap-2">
                <Icon :icon="item.icon" class="w-4 h-4 lg:w-5 lg:h-5" />
                <span>{{ item.label }}</span>
              </div>
              <!-- 激活指示器 -->
              <div
                v-if="isActiveRoute(item.path)"
                class="absolute inset-0 bg-blue-500/10 dark:bg-blue-400/10 rounded-xl -z-10" />
              <div
                v-if="isActiveRoute(item.path)"
                class="absolute bottom-0 left-1/2 -translate-x-1/2 w-8 h-0.5 bg-blue-600 dark:bg-blue-400 rounded-full" />
            </router-link>
          </nav>

          <!-- 移动端菜单按钮 -->
          <button
            @click="mobileMenuOpen = !mobileMenuOpen"
            class="md:hidden w-10 h-10 rounded-xl flex items-center justify-center hover:bg-[--hover-bg] transition-all duration-200 active:scale-95"
            aria-label="菜单">
            <Icon
              :icon="mobileMenuOpen ? 'solar:close-circle-bold' : 'solar:hamburger-menu-bold'"
              class="w-6 h-6 text-[--text-primary]" />
          </button>
        </div>
      </div>

      <!-- 移动端导航菜单 -->
      <Transition
        enter-active-class="transition-all duration-300 ease-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition-all duration-200 ease-in"
        leave-from-class="opacity-100 translate-y-0"
        leave-to-class="opacity-0 -translate-y-2">
        <div v-if="mobileMenuOpen" class="md:hidden border-t border-[--border-primary]">
          <nav class="max-w-7xl mx-auto px-4 py-3 flex flex-col gap-1">
            <router-link
              v-for="item in navItems"
              :key="item.path"
              :to="item.path"
              @click="mobileMenuOpen = false"
              class="flex items-center gap-3 px-4 py-3 rounded-xl transition-all duration-200 active:scale-[0.98]"
              :class="isActiveRoute(item.path)
                ? 'bg-blue-500/10 text-blue-600 dark:text-blue-400'
                : 'text-[--text-secondary] hover:bg-[--hover-bg] hover:text-[--text-primary]'">
              <Icon :icon="item.icon" class="w-5 h-5" />
              <span class="font-medium">{{ item.label }}</span>
              <Icon
                v-if="isActiveRoute(item.path)"
                icon="solar:check-circle-bold"
                class="w-5 h-5 ml-auto" />
            </router-link>
          </nav>
        </div>
      </Transition>
    </header>

    <!-- 主内容区域 -->
    <main class="flex-1 w-full">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8 lg:py-10">
        <Transition
          mode="out-in"
          enter-active-class="transition-all duration-300 ease-out"
          enter-from-class="opacity-0 translate-y-4"
          enter-to-class="opacity-100 translate-y-0"
          leave-active-class="transition-all duration-200 ease-in"
          leave-from-class="opacity-100 translate-y-0"
          leave-to-class="opacity-0 -translate-y-4">
          <router-view v-slot="{ Component }">
            <component :is="Component" />
          </router-view>
        </Transition>
      </div>
    </main>

    <!-- 底部信息 -->
    <footer class="border-t border-[--border-primary] bg-[--bg-secondary]">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
        <div class="flex flex-col sm:flex-row items-center justify-between gap-4">
          <div class="flex items-center gap-2 text-[--text-tertiary] text-sm">
            <Icon icon="solar:copyright-bold" class="w-4 h-4" />
            <span>{{ currentYear }} 建大导航</span>
          </div>
          <p class="text-[--text-tertiary] text-sm text-center sm:text-right">
            简约高效的校园链接管理平台
          </p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { Icon } from '@iconify/vue'

// 导航项接口
interface NavItem {
  path: string
  label: string
  icon: string
}

// 导航配置
const navItems: NavItem[] = [
  { 
    path: '/', 
    label: '首页', 
    icon: 'solar:home-2-bold-duotone' 
  },
  { 
    path: '/categories', 
    label: '分类管理', 
    icon: 'solar:widget-5-bold-duotone' 
  },
  { 
    path: '/users', 
    label: '用户管理', 
    icon: 'solar:users-group-rounded-bold-duotone' 
  },
  { 
    path: '/data', 
    label: '数据管理', 
    icon: 'solar:database-bold-duotone' 
  }
]

// 响应式状态
const mobileMenuOpen = ref(false)
const route = useRoute()

// 当前年份
const currentYear = computed(() => new Date().getFullYear())

// 判断路由是否激活
const isActiveRoute = (path: string): boolean => {
  if (path === '/') {
    return route.path === '/' || route.path === ''
  }
  return route.path === path || route.path.startsWith(`${path}/`)
}
</script>

<style scoped>
/* CSS 变量 - 支持亮色和暗色模式 */
:root {
  --bg-primary: #ffffff;
  --bg-secondary: #f9fafb;
  --glass-bg: rgba(255, 255, 255, 0.8);
  --text-primary: #111827;
  --text-secondary: #6b7280;
  --text-tertiary: #9ca3af;
  --border-primary: rgba(0, 0, 0, 0.08);
  --hover-bg: rgba(0, 0, 0, 0.04);
}

@media (prefers-color-scheme: dark) {
  :root {
    --bg-primary: #000000;
    --bg-secondary: #0a0a0a;
    --glass-bg: rgba(0, 0, 0, 0.7);
    --text-primary: #f9fafb;
    --text-secondary: #9ca3af;
    --text-tertiary: #6b7280;
    --border-primary: rgba(255, 255, 255, 0.1);
    --hover-bg: rgba(255, 255, 255, 0.06);
  }
}

/* 如果使用类名切换暗色模式 */
:global(.dark) {
  --bg-primary: #000000;
  --bg-secondary: #0a0a0a;
  --glass-bg: rgba(0, 0, 0, 0.7);
  --text-primary: #f9fafb;
  --text-secondary: #9ca3af;
  --text-tertiary: #6b7280;
  --border-primary: rgba(255, 255, 255, 0.1);
  --hover-bg: rgba(255, 255, 255, 0.06);
}

/* 自定义滚动条 - 苹果风格 */
:deep(*::-webkit-scrollbar) {
  width: 8px;
  height: 8px;
}

:deep(*::-webkit-scrollbar-track) {
  background: transparent;
}

:deep(*::-webkit-scrollbar-thumb) {
  background: var(--text-tertiary);
  border-radius: 4px;
  transition: background 0.2s ease;
}

:deep(*::-webkit-scrollbar-thumb:hover) {
  background: var(--text-secondary);
}

/* 平滑滚动 */
* {
  scroll-behavior: smooth;
}

/* 响应式断点扩展 */
@media (min-width: 475px) {
  .xs\:block {
    display: block;
  }
}
</style>