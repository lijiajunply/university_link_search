<template>
  <div class="min-h-screen flex flex-col">
    <!-- 顶部导航栏 -->
    <header
        class="sticky top-0 z-50 backdrop-blur-md bg-white/80 dark:bg-gray-900/80 border-b border-gray-200 dark:border-gray-800">
      <div class="container mx-auto px-4 py-3 flex items-center justify-between">
        <!-- 左侧Logo和标题 -->
        <div class="flex items-center gap-2">
          <h1 class="text-xl font-semibold text-gray-900 dark:text-white">建大导航</h1>
        </div>

        <!-- 右侧操作区 -->
        <div>
          <!-- 导航链接 - 桌面端 -->
          <nav class="hidden md:flex items-center space-x-6">
            <router-link
                v-for="item in navItems"
                :key="item.path"
                :to="item.path"
                class="text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 text-sm font-medium transition-colors"
                :class="{ 'text-blue-600 dark:text-blue-400': isActiveRoute(item.path) }"
            >
              {{ item.label }}
            </router-link>
          </nav>

          <!-- 移动端菜单按钮 -->
          <button
              @click="mobileMenuOpen = !mobileMenuOpen"
              class="md:hidden p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-800"
              aria-label="菜单"
          >
            <Icon :icon="mobileMenuOpen ? 'mdi:close' : 'mdi:menu'" class="h-5 w-5 text-gray-700 dark:text-gray-300"/>
          </button>
        </div>
      </div>
    </header>

    <!-- 移动端菜单 -->
    <div v-if="mobileMenuOpen"
         class="md:hidden bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-800">
      <div class="container mx-auto px-4 py-2">
        <nav class="flex flex-col space-y-2 py-2">
          <router-link
              v-for="item in navItems"
              :key="item.path"
              :to="item.path"
              class="px-3 py-2 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-800"
              :class="{ 'bg-gray-100 dark:bg-gray-800 text-blue-600 dark:text-blue-400': isActiveRoute(item.path) }"
              @click="mobileMenuOpen = false"
          >
            {{ item.label }}
          </router-link>
        </nav>
      </div>
    </div>

    <!-- 主内容区 -->
    <main class="flex-1 container mx-auto px-4 py-6 w-full">
      <router-view/>
    </main>
  </div>
</template>

<script setup lang="ts">
import {ref} from 'vue';
import {useRoute} from 'vue-router';
import {Icon} from '@iconify/vue';

const route = useRoute();

// 导航项 - 根据router.ts中的路由配置动态生成
const navItems = [
  {path: '/', label: '首页'},
  {path: '/categories', label: '分类管理'},
  {path: '/users', label: '用户管理'},
  {path: '/data', label: '数据管理'}
];

// 移动端菜单状态
const mobileMenuOpen = ref(false);

// 检查当前路由是否匹配导航项，处理根路径冲突
const isActiveRoute = (path: string): boolean => {
  // 处理根路径特殊情况
  if (path === '/' && route.path === '') {
    return true;
  }
  return route.path === path;
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