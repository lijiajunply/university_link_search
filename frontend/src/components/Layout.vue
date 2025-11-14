<template>
  <div class="min-h-screen flex flex-col">
    <!-- 顶部导航栏 -->
    <header class="sticky top-0 z-50 backdrop-blur-md bg-white/80 dark:bg-gray-900/80 border-b border-gray-200 dark:border-gray-800">
      <div class="container mx-auto px-4 py-3 flex items-center justify-between">
        <!-- 左侧Logo和标题 -->
        <div class="flex items-center gap-2">
          <div class="h-8 w-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
            <Icon icon="mdi:link-variant" class="text-white" />
          </div>
          <h1 class="text-xl font-semibold text-gray-900 dark:text-white">建大导航</h1>
        </div>

        <!-- 中间导航链接 - 桌面端 -->
        <nav class="hidden md:flex items-center space-x-6">
          <router-link
            v-for="item in navItems"
            :key="item.path"
            :to="item.path"
            class="text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 text-sm font-medium transition-colors"
            :class="{ 'text-blue-600 dark:text-blue-400': $route.path === item.path }"
          >
            {{ item.label }}
          </router-link>
        </nav>

        <!-- 右侧操作区 -->
        <div class="flex items-center gap-3">
          <!-- 主题切换按钮 -->
          <button
            @click="toggleTheme"
            class="p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
            aria-label="切换主题"
          >
            <Icon :icon="isDark ? 'mdi:weather-sunny' : 'mdi:weather-night'" class="h-5 w-5 text-gray-700 dark:text-gray-300" />
          </button>

          <!-- 用户头像/登录按钮 -->
          <button
            v-if="!isLoggedIn"
            @click="navigateToLogin"
            class="px-4 py-1.5 rounded-full bg-blue-600 text-white text-sm font-medium hover:bg-blue-700 transition-colors"
          >
            登录
          </button>
          <div v-else class="relative group">
            <button class="flex items-center gap-2 px-2 py-1 rounded-full hover:bg-gray-100 dark:hover:bg-gray-800">
              <div class="h-8 w-8 rounded-full bg-gradient-to-r from-indigo-500 to-purple-600 flex items-center justify-center text-white text-sm">
                {{ userInitial }}
              </div>
            </button>
          </div>

          <!-- 移动端菜单按钮 -->
          <button
            @click="mobileMenuOpen = !mobileMenuOpen"
            class="md:hidden p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-800"
            aria-label="菜单"
          >
            <Icon :icon="mobileMenuOpen ? 'mdi:close' : 'mdi:menu'" class="h-5 w-5 text-gray-700 dark:text-gray-300" />
          </button>
        </div>
      </div>
    </header>

    <!-- 移动端菜单 -->
    <div v-if="mobileMenuOpen" class="md:hidden bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-800">
      <div class="container mx-auto px-4 py-2">
        <nav class="flex flex-col space-y-2 py-2">
          <router-link
            v-for="item in navItems"
            :key="item.path"
            :to="item.path"
            class="px-3 py-2 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-800"
            :class="{ 'bg-gray-100 dark:bg-gray-800 text-blue-600 dark:text-blue-400': $route.path === item.path }"
            @click="mobileMenuOpen = false"
          >
            {{ item.label }}
          </router-link>
        </nav>
      </div>
    </div>

    <!-- 主内容区 -->
    <main class="flex-1 container mx-auto px-4 py-6">
      <slot></slot>
    </main>

    <!-- 底部 -->
    <footer class="border-t border-gray-200 dark:border-gray-800 py-6">
      <div class="container mx-auto px-4 text-center text-gray-500 dark:text-gray-400 text-sm">
        <p>建大导航 © 2024</p>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { Icon } from '@iconify/vue';

// Props
interface Props {
  isDark?: boolean;
  toggleTheme?: () => void;
}

const props = withDefaults(defineProps<Props>(), {
  isDark: false,
  toggleTheme: () => {}
});

// 导航项
const navItems = [
  { path: '/', label: '首页' },
  { path: '/links', label: '链接管理' },
  { path: '/categories', label: '分类管理' },
  { path: '/users', label: '用户管理' },
  { path: '/data', label: '数据管理' }
];

// 移动端菜单状态
const mobileMenuOpen = ref(false);

// 路由
const router = useRouter();

// 用户状态（模拟）
const isLoggedIn = ref(false);
const userInitial = computed(() => isLoggedIn.value ? 'U' : '');

// 导航到登录页
const navigateToLogin = () => {
  router.push('/login');
};
</script>

<style scoped>
/* 滚动条样式 - 苹果风格 */
:deep(::-webkit-scrollbar) {
  width: 8px;
  height: 8px;
}

:deep(::-webkit-scrollbar-track) {
  background: transparent;
}

:deep(::-webkit-scrollbar-thumb) {
  background: #ccc;
  border-radius: 4px;
}

:deep(::-webkit-scrollbar-thumb:hover) {
  background: #aaa;
}

.dark :deep(::-webkit-scrollbar-thumb) {
  background: #555;
}

.dark :deep(::-webkit-scrollbar-thumb:hover) {
  background: #777;
}
</style>