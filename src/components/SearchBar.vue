<template>
  <!-- 原始组件 -->
  <div ref="originalNav" class="pt-12 pb-8">
    <div class="container mx-auto px-4">
      <div class="flex items-center justify-center mt-8">
        <div class="bg-white/10 backdrop-blur-sm rounded-lg px-4 py-2 flex items-center space-x-2">
          <h1 class="text-4xl font-bold text-white">
            {{time}}
          </h1>
        </div>
      </div>

      <div class="max-w-2xl mx-auto mt-8">
        <div class="bg-white/90 backdrop-blur-sm rounded-full shadow-lg flex items-center px-4 py-3">
          <!-- 搜索引擎选择器 -->
          <div class="relative">
            <button
                @click="showEngines = !showEngines"
                class="flex items-center space-x-2 px-3 py-1 rounded-full hover:bg-gray-100 transition-colors"
            >
              <img :src="currentEngine.icon" :alt="currentEngine.name" class="w-5 h-5">
              <svg class="w-4 h-4 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
              </svg>
            </button>

            <!-- 搜索引擎下拉菜单 -->
            <div v-if="showEngines" class="absolute top-full left-0 mt-2 bg-white rounded-lg shadow-lg py-2 z-50">
              <button
                  v-for="engine in searchEngines"
                  :key="engine.id"
                  @click="selectEngine(engine)"
                  class="flex items-center space-x-2 px-4 py-2 hover:bg-gray-100 w-full text-left"
              >
                <img :src="engine.icon" :alt="engine.name" class="w-5 h-5">
                <span class="text-gray-700">{{ engine.name }}</span>
              </button>
            </div>
          </div>

          <!-- 搜索输入框 -->
          <input
              v-model="searchQuery"
              @keyup.enter="search"
              type="text"
              :placeholder="`在 ${currentEngine.name} 中搜索`"
              class="flex-1 px-4 py-1 outline-none bg-transparent"
          >

          <!-- 搜索按钮 -->
          <button
              @click="search"
              class="bg-blue-500 text-white p-2 rounded-full hover:bg-blue-600 transition-colors flex items-center space-x-2"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
          </button>
        </div>
      </div>
    </div>
  </div>

  <!-- 固定在顶部的左右格式版本 -->
  <div v-if="isSticky" class="fixed top-0 left-0 right-0 bg-white/30 backdrop-blur-md shadow-md py-2 z-50 transition-all duration-300 transform translate-y-0">
    <div class="container mx-auto px-4">
      <div class="flex items-center justify-between">
        <!-- 左侧时间显示 -->
        <div class="flex items-center">
          <div class="bg-white/40 backdrop-blur-sm rounded-lg px-3 py-1">
            <h1 class="text-xl font-bold text-gray-600">{{time}}</h1>
          </div>
        </div>

        <!-- 右侧搜索框 -->
        <div class="w-1/2">
          <div class="bg-white/90 backdrop-blur-sm rounded-full shadow-lg flex items-center px-3 py-2">
            <!-- 搜索引擎选择器 -->
            <div class="relative">
              <button
                  @click="showEngines = !showEngines"
                  class="flex items-center space-x-1 px-2 py-1 rounded-full hover:bg-gray-100 transition-colors"
              >
                <img :src="currentEngine.icon" :alt="currentEngine.name" class="w-4 h-4">
                <svg class="w-3 h-3 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
                </svg>
              </button>

              <!-- 搜索引擎下拉菜单 -->
              <div v-if="showEngines" class="absolute top-full left-0 mt-2 bg-white rounded-lg shadow-lg py-2 z-50">
                <button
                    v-for="engine in searchEngines"
                    :key="engine.id"
                    @click="selectEngine(engine)"
                    class="flex items-center space-x-2 px-4 py-2 hover:bg-gray-100 w-full text-left"
                >
                  <img :src="engine.icon" :alt="engine.name" class="w-5 h-5">
                  <span class="text-gray-700">{{ engine.name }}</span>
                </button>
              </div>
            </div>

            <!-- 搜索输入框 -->
            <input
                v-model="searchQuery"
                @keyup.enter="search"
                type="text"
                :placeholder="`在 ${currentEngine.name} 中搜索`"
                class="flex-1 px-3 py-1 outline-none bg-transparent text-sm"
            >

            <!-- 搜索按钮 -->
            <button
                @click="search"
                class="bg-blue-500 text-white p-1.5 rounded-full hover:bg-blue-600 transition-colors flex items-center"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import {onMounted, onUnmounted, ref} from 'vue'

const searchQuery = ref('')
const showEngines = ref(false)
const time = ref('')

const searchEngines = [
  {
    id: 1,
    name: '百度',
    icon: 'https://www.baidu.com/favicon.ico',
    searchUrl: 'https://www.baidu.com/s?wd='
  },
  {
    id: 2,
    name: '谷歌',
    icon: 'https://www.google.com/favicon.ico',
    searchUrl: 'https://www.google.com/search?q='
  },
  {
    id: 3,
    name: '必应',
    icon: 'https://www.bing.com/favicon.ico',
    searchUrl: 'https://www.bing.com/search?q='
  },
  {
    id: 4,
    name: 'DuckDuckGo',
    icon: 'https://duckduckgo.com/favicon.ico',
    searchUrl: 'https://duckduckgo.com/?q='
  }
]

// 新增的状态和引用
const isSticky = ref(false);
const originalNav = ref(null);
const currentEngine = ref(searchEngines[0])

const updateTime = function () {
  const now = new Date();
  // 获取小时和分钟
  const hours = now.getHours().toString().padStart(2, '0');
  const minutes = now.getMinutes().toString().padStart(2, '0');
  // 格式化时间为 'hh:mm'
  time.value = `${hours}:${minutes}`;
};


const selectEngine = (engine) => {
  currentEngine.value = engine
  showEngines.value = false
}

const search = () => {
  if (searchQuery.value.trim()) {
    window.open(currentEngine.value.searchUrl + encodeURIComponent(searchQuery.value), '_blank')
  }
}

// 滚动监听函数
const handleScroll = () => {
  if (!originalNav.value) return;

  const rect = originalNav.value.getBoundingClientRect();
  // 当原始导航完全离开视口顶部时，激活固定导航
  isSticky.value = rect.bottom <= 0;
};

// 添加和移除滚动事件监听
onMounted(() => {
  window.addEventListener('scroll', handleScroll);
  updateTime();
  // 每分钟更新一次时间
  setInterval(updateTime, 60000);
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
});
</script>