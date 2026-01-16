<template>
  <div class="min-h-screen flex flex-col bg-[#F5F5F7] dark:bg-[#000000] transition-colors duration-500 font-sans selection:bg-blue-500/30">
    <!-- 顶部导航栏 - macOS/iOS 风格 -->
    <header class="sticky top-0 z-50 w-full">
      <!-- 毛玻璃背景 -->
      <div class="absolute inset-0 bg-white/70 dark:bg-[#1C1C1E]/70 backdrop-blur-xl border-b border-gray-200/50 dark:border-white/10 transition-colors duration-500"></div>
      
      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between h-14 sm:h-16">
          <!-- Logo 区域 -->
          <router-link to="/" class="flex items-center gap-3 group relative z-10">
            <div class="w-9 h-9 rounded-xl bg-gradient-to-br from-[#007AFF] to-[#0055FF] flex items-center justify-center shadow-lg shadow-blue-500/20 group-hover:scale-105 group-active:scale-95 transition-all duration-300">
              <Icon icon="solar:home-smile-bold" class="w-5 h-5 text-white" />
            </div>
            <span class="text-lg font-semibold tracking-tight text-gray-900 dark:text-white hidden xs:block">
              建大导航
            </span>
          </router-link>

          <!-- 桌面端导航 - 胶囊风格 -->
          <nav class="hidden md:flex items-center p-1 bg-gray-100/80 dark:bg-[#2C2C2E]/80 backdrop-blur-md rounded-full border border-gray-200/50 dark:border-white/5 shadow-inner">
            <router-link
              v-for="item in navItems"
              :key="item.path"
              :to="item.path"
              class="relative px-4 py-1.5 rounded-full text-sm font-medium transition-all duration-300 ease-out group"
              :class="isActiveRoute(item.path) 
                ? 'text-gray-900 dark:text-white bg-white dark:bg-[#636366] shadow-[0_2px_8px_rgba(0,0,0,0.08)] dark:shadow-[0_2px_8px_rgba(0,0,0,0.2)]' 
                : 'text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white'"
            >
              <div class="flex items-center gap-2">
                <Icon :icon="item.icon" class="w-4 h-4 transition-colors" :class="{ 'text-[#007AFF] dark:text-[#0A84FF]': isActiveRoute(item.path) }" />
                <span>{{ item.label }}</span>
              </div>
            </router-link>
          </nav>

          <!-- 右侧工具栏 -->
          <div class="flex items-center gap-3 relative z-10">
            <!-- 主题切换 -->
            <button 
              @click="toggleTheme"
              class="w-9 h-9 rounded-full flex items-center justify-center transition-all duration-300 bg-gray-100/50 dark:bg-[#2C2C2E] hover:bg-gray-200/50 dark:hover:bg-[#3A3A3C] text-gray-600 dark:text-gray-300 active:scale-90"
              aria-label="切换主题"
            >
              <Icon 
                :icon="isDark ? 'solar:moon-bold' : 'solar:sun-bold'" 
                class="w-5 h-5 transition-transform duration-500 rotate-0" 
                :class="{ 'rotate-[360deg] text-yellow-500': !isDark, 'text-blue-400': isDark }" 
              />
            </button>

            <!-- 移动端菜单按钮 -->
            <button
              @click="mobileMenuOpen = !mobileMenuOpen"
              class="md:hidden w-9 h-9 rounded-full flex items-center justify-center bg-gray-100/50 dark:bg-[#2C2C2E] text-gray-900 dark:text-white active:scale-90 transition-all"
            >
              <Icon :icon="mobileMenuOpen ? 'solar:close-circle-bold' : 'solar:hamburger-menu-bold'" class="w-5 h-5" />
            </button>
          </div>
        </div>
      </div>

      <!-- 移动端菜单 -->
      <Transition
        enter-active-class="transition duration-300 ease-out"
        enter-from-class="transform -translate-y-4 opacity-0"
        enter-to-class="transform translate-y-0 opacity-100"
        leave-active-class="transition duration-200 ease-in"
        leave-from-class="transform translate-y-0 opacity-100"
        leave-to-class="transform -translate-y-4 opacity-0"
      >
        <div 
          v-if="mobileMenuOpen" 
          class="md:hidden absolute top-full left-0 w-full bg-white/90 dark:bg-[#1C1C1E]/95 backdrop-blur-2xl border-b border-gray-200/50 dark:border-white/10 shadow-2xl origin-top"
        >
          <nav class="px-4 py-4 space-y-2">
            <router-link
              v-for="item in navItems"
              :key="item.path"
              :to="item.path"
              @click="mobileMenuOpen = false"
              class="flex items-center gap-3 px-4 py-3.5 rounded-2xl transition-all duration-200"
              :class="isActiveRoute(item.path)
                ? 'bg-[#007AFF]/10 text-[#007AFF] dark:text-[#0A84FF] font-semibold'
                : 'text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-[#2C2C2E]'"
            >
              <Icon :icon="item.icon" class="w-5 h-5" />
              <span>{{ item.label }}</span>
              <Icon v-if="isActiveRoute(item.path)" icon="solar:check-circle-bold" class="w-4 h-4 ml-auto" />
            </router-link>
          </nav>
        </div>
      </Transition>
    </header>

    <!-- 主内容区域 -->
    <main class="flex-1 w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 sm:py-10">
      <router-view v-slot="{ Component }">
        <Transition
          enter-active-class="transition-all duration-500 ease-out"
          enter-from-class="opacity-0 translate-y-4 scale-[0.98] blur-sm"
          enter-to-class="opacity-100 translate-y-0 scale-100 blur-0"
          leave-active-class="transition-all duration-300 ease-in"
          leave-from-class="opacity-100 translate-y-0 scale-100 blur-0"
          leave-to-class="opacity-0 -translate-y-4 scale-[0.98] blur-sm"
          mode="out-in"
        >
          <component :is="Component" />
        </Transition>
      </router-view>
    </main>

    <!-- 底部 -->
    <footer class="border-t border-gray-200/50 dark:border-white/5 bg-white/50 dark:bg-[#1C1C1E]/50 backdrop-blur-lg mt-auto">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div class="flex flex-col md:flex-row items-center justify-between gap-4">
          <div class="flex items-center gap-2 text-gray-400 dark:text-gray-600 text-sm">
            <Icon icon="solar:copyright-bold" class="w-4 h-4" />
            <span>{{ currentYear }} 建大导航</span>
          </div>
          <div class="text-sm text-gray-400 dark:text-gray-600 flex gap-4">
            <span class="hover:text-gray-600 dark:hover:text-gray-400 transition-colors cursor-pointer">关于我们</span>
            <span class="hover:text-gray-600 dark:hover:text-gray-400 transition-colors cursor-pointer">免责声明</span>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { Icon } from '@iconify/vue'
import { useOsTheme } from 'naive-ui'

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
    label: '分类', 
    icon: 'solar:widget-5-bold-duotone' 
  }
]

// 响应式状态
const mobileMenuOpen = ref(false)
const route = useRoute()
const currentYear = computed(() => new Date().getFullYear())
const isDark = ref(false)

// 主题切换逻辑
const toggleTheme = () => {
  isDark.value = !isDark.value
  updateThemeClass()
}

const updateThemeClass = () => {
  const htmlEl = document.documentElement
  if (isDark.value) {
    htmlEl.classList.add('dark')
  } else {
    htmlEl.classList.remove('dark')
  }
  // 保存偏好
  localStorage.setItem('theme', isDark.value ? 'dark' : 'light')
}

// 初始化主题
onMounted(() => {
  const savedTheme = localStorage.getItem('theme')
  const osTheme = useOsTheme()
  
  if (savedTheme) {
    isDark.value = savedTheme === 'dark'
  } else {
    isDark.value = osTheme.value === 'dark'
  }
  updateThemeClass()
})

// 判断路由是否激活
const isActiveRoute = (path: string): boolean => {
  if (path === '/') {
    return route.path === '/' || route.path === ''
  }
  return route.path === path || route.path.startsWith(`${path}/`)
}
</script>

<style scoped>
/* 隐藏滚动条但保留功能 - 适用于 Webkit 浏览器 */
:deep(::-webkit-scrollbar) {
  width: 0px;
  background: transparent;
}

/* 确保平滑滚动 */
html {
  scroll-behavior: smooth;
}
</style>