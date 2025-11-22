<template>
  <div class="min-h-screen flex flex-col bg-white">
      <!-- 顶部导航栏 - 苹果风格磨砂效果 -->
      <header
        class="sticky top-0 z-50 backdrop-blur-lg bg-white/90 border-b border-gray-100 shadow-sm transition-all duration-300">
        <div class="container mx-auto px-4 py-3 flex items-center justify-between">
          <!-- 左侧Logo和标题 - 响应式设计 -->
          <div class="flex items-center gap-3">
            <Icon icon="logos:apple" class="h-6 w-6 text-black dark:text-white" />
            <h1 class="text-xl font-semibold tracking-tight text-gray-900 dark:text-white sm:text-2xl">建大导航</h1>
          </div>

          <!-- 右侧操作区 -->
          <div class="flex items-center gap-4">
            <!-- 导航链接 - 响应式设计，添加更多断点 -->
            <nav class="hidden md:flex items-center space-x-6 lg:space-x-8">
              <router-link v-for="item in navItems" :key="item.path" :to="item.path"
                class="relative text-gray-700 hover:text-blue-500 text-sm md:text-base font-medium transition-colors duration-200 flex items-center gap-2"
                :class="{ 'text-blue-500': isActiveRoute(item.path) }">
                <!-- 桌面端导航图标 - 苹果风格 -->
                <Icon :icon="getAppleStyleIcon(item.path)" class="h-4 w-4 md:h-5 md:w-5 text-gray-700" />
                {{ item.label }}
                <!-- 苹果风格下划线指示器 -->
                <span v-if="isActiveRoute(item.path)"
                  class="absolute bottom-[-6px] left-0 right-0 h-0.5 bg-blue-500 dark:bg-blue-400 rounded-full transition-all duration-300"></span>
              </router-link>
            </nav>



            <!-- 移动端菜单按钮 - 适配不同尺寸的触摸目标 -->
            <button @click="mobileMenuOpen = !mobileMenuOpen"
              class="md:hidden p-2 rounded-full hover:bg-gray-100 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 min-w-[44px] min-h-[44px] flex items-center justify-center"
              aria-label="菜单">
              <Icon :icon="mobileMenuOpen ? 'mdi:close' : 'mdi:menu'" class="h-6 w-6 text-gray-700" />
            </button>
          </div>
        </div>
      </header>

      <!-- 移动端菜单 - 苹果风格卡片效果 -->
      <div v-if="mobileMenuOpen"
        class="md:hidden bg-white border-b border-gray-100 shadow-md animate-in slide-in-from-top-5 duration-200">
        <div class="container mx-auto px-4 py-2">
          <nav class="flex flex-col space-y-1 py-2">
            <router-link v-for="item in navItems" :key="item.path" :to="item.path"
              class="px-4 py-3 rounded-xl text-gray-700 hover:bg-gray-50 transition-all duration-200 flex items-center gap-3"
              :class="{ 'bg-gray-50 text-blue-500': isActiveRoute(item.path) }" @click="mobileMenuOpen = false">
              <!-- 移动端导航图标 - 苹果风格 -->
              <Icon :icon="getAppleStyleIcon(item.path)" class="h-6 w-6 flex-shrink-0" />
              {{ item.label }}
            </router-link>
          </nav>
        </div>
      </div>

      <!-- 主内容区 - 苹果风格内容容器，响应式边距和内边距 -->
      <main class="flex-1 container mx-auto px-4 py-6 sm:py-8 md:py-10 w-full">
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden transition-all duration-300">
          <router-view />
        </div>
      </main>

      <!-- 底部信息栏 -->
      <footer class="bg-white border-t border-gray-100 py-4">
        <div class="container mx-auto px-4 text-center text-sm text-gray-500">
          <p>© {{ new Date().getFullYear() }} 建大导航 - 简约高效的校园链接管理平台</p>
        </div>
      </footer>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { Icon } from '@iconify/vue';


// 响应式引用
const mobileMenuOpen = ref(false);

// 组合式API
const route = useRoute();

// 导航项配置 - 使用苹果系统风格图标
interface NavItem {
  path: string;
  label: string;
  icon: string; // @iconify/vue 图标名称
  appleIcon?: string; // 特定的苹果风格图标
}

const navItems: NavItem[] = [
  { path: '/', label: '首页', icon: 'lucide:home', appleIcon: 'logos:apple' },
  { path: '/categories', label: '分类管理', icon: 'lucide:grid', appleIcon: 'icon-park:category' },
  { path: '/users', label: '用户管理', icon: 'lucide:users', appleIcon: 'material-symbols:people-outline' },
  { path: '/data', label: '数据管理', icon: 'lucide:database', appleIcon: 'material-symbols:storage-outline' }
];

// 方法
// 检查当前路由是否激活 - 增强类型安全
const isActiveRoute = (path: string): boolean => {
  // 处理根路径特殊情况
  if (path === '/' && (route.path === '' || route.path === '/')) {
    return true;
  }
  // 精确匹配或子路径匹配
  return route.path === path || route.path.startsWith(`${path}/`);
};

// 获取苹果风格的图标 - 明确返回类型
const getAppleStyleIcon = (path: string): string => {
  const item = navItems.find(item => item.path === path);
  return item?.appleIcon || item?.icon || 'lucide:help-circle';
};
</script>

<style scoped>
/* 滚动条样式 - 苹果风格优化 */
:deep(::-webkit-scrollbar) {
  width: 10px;
  height: 10px;
}

:deep(::-webkit-scrollbar-track) {
  background: transparent;
}

:deep(::-webkit-scrollbar-thumb) {
  background: rgba(156, 163, 175, 0.5);
  border-radius: 5px;
  transition: background 0.2s ease;
}

:deep(::-webkit-scrollbar-thumb:hover) {
  background: rgba(156, 163, 175, 0.7);
}

/* 全局动画 */
@keyframes fadeIn {
  from {
    opacity: 0;
  }

  to {
    opacity: 1;
  }
}

.animate-in {
  animation: fadeIn 0.2s ease-out;
}

.slide-in-from-top-5 {
  transform: translateY(-20px);
  animation: slideInTop 0.3s ease-out forwards;
}

@keyframes slideInTop {
  to {
    transform: translateY(0);
  }
}
</style>