<template>
  <!-- 原始组件 -->
  <div ref="originalNav" class="pt-12 pb-8 z-9999">
    <div class="container mx-auto z-9999">
      <div class="grid grid-cols-3 gap-4 mt-8">
        <div></div>
        <div class="flex justify-center">
          <div class="bg-white/10 dark:bg-black/10 backdrop-blur-sm rounded-lg px-4 py-2 space-x-2">
            <h1 class="text-4xl font-bold text-white dark:text-white/80">
              {{ time }}
            </h1>
          </div>
        </div>
        <div>
          <div class="flex justify-end" v-if="isShowSetting">
            <div class="relative">
              <div @click="isOpenLogin = true" @mouseenter="isOpenAccount = true" @mouseleave="isOpenAccount = false"
                class="flex items-center space-x-2 mx-2 p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-700/85 transition-colors">
                <Icon class="text-gray-600 dark:text-gray-300 text-2xl">
                  <AccountCircleRound />
                </Icon>
              </div>
              <div class="absolute top-full right-1 mt-2 py-2 z-50" v-show="isOpenAccount"
                @mouseenter="isOpenAccount = true" @mouseleave="isOpenAccount = false">
                <AccountCard />
              </div>
            </div>
            <div @click="isOpenSetting = !isOpenSetting" id="setting"
              class="flex items-center space-x-2 mx-2 p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-700/85 transition-colors">
              <Icon class="text-gray-600 dark:text-gray-300 text-2xl">
                <Settings16Filled />
              </Icon>
            </div>
          </div>
        </div>
      </div>

      <div class="mx-8 md:mx-auto mt-8 md:max-w-2xl z-9999">
        <div
          class="bg-white/90 dark:bg-black/30 backdrop-blur-sm rounded-full shadow-lg flex items-center px-4 py-3 z-9999">
          <!-- 搜索引擎选择器 -->
          <div class="relative z-9999" id="chooseEngine">
          <button @click.stop="showEngines = !showEngines"
            class="flex items-center space-x-2 px-3 py-1 rounded-full dark:hover:bg-transparent hover:bg-gray-100 transition-colors">
            <component :is="currentEngine.icon()" />
            <svg class="w-4 h-4 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
            </svg>
          </button>

            <!-- 搜索引擎下拉菜单 -->
            <div v-if="showEngines" :class="[showEngines ? 'fade-in-scale' : 'fade-out-scale']"
              class="absolute top-full left-0 mt-4 bg-white dark:bg-black/60 rounded-lg shadow-lg py-2 z-9999">
              <button v-for="engine in searchEngines" :key="engine.key" @click="selectEngine(engine)"
                class="flex items-center space-x-2 px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-900 w-full text-left">
                <component :is="engine.icon()" class="w-5 h-5" />
                <span class="text-gray-700 dark:text-white/90">{{ engine.label }}</span>
              </button>
            </div>
          </div>

          <!-- 搜索输入框 -->
          <div class="relative flex-1 px-4 py-1">
            <input v-model="searchQuery" @keyup.enter="search" @keyup="getSuggestions" type="text"
              :placeholder="`在 ${currentEngine.label} 中搜索`"
              class="outline-none bg-transparent dark:text-gray-100 w-full">
            <div class="absolute left-0 mt-4 bg-white/70 dark:bg-black/50 backdrop-blur-sm rounded-lg shadow-lg z-9999 "
              v-if="isOpenSuggestions">
              <button @click="transform"
                class="flex items-center space-x-2 px-4 py-2 dark:hover:bg-gray-800 hover:bg-gray-100 w-full text-left rounded-lg">
                <span class="text-gray-700 text-sm dark:text-gray-100">翻译: {{ searchQuery }}</span>
              </button>
              <button v-for="(suggestion, index) in suggestions" @click="openUrl(suggestion)"
                :class="{ 'bg-gray-100': index === selectedSuggestionIndex }"
                class="flex items-center space-x-2 px-4 py-2 dark:hover:bg-gray-800 hover:bg-gray-100 w-full text-left rounded-lg">
                <span class="text-gray-700 text-sm dark:text-gray-100">{{ suggestion }}</span>
              </button>
            </div>
          </div>

          <!-- 搜索按钮 -->
          <button @click="search"
            class="dark:hover:bg-blue-600/80 dark:bg-blue-500/80 bg-blue-500 text-white p-2 rounded-full hover:bg-blue-600 transition-colors flex items-center space-x-2 desktop-phone">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
          </button>
        </div>
      </div>
    </div>
  </div>

  <!-- 固定在顶部的左右格式版本 -->
  <div :class="firstSticky ? 'opacity-0' : isSticky ? 'liquid-card-in' : 'liquid-card-out'"
    class="fixed top-0 left-0 right-0 py-2 z-50 transition-all duration-300 transform translate-y-0 mx-6 my-4 rounded-xl liquid-card">
    <div class="mx-auto px-4">
      <div class="flex items-center justify-between">
        <!-- 左侧时间显示 -->
        <div class="flex items-center">
          <div class="bg-white/40 backdrop-blur-sm rounded-lg px-3 py-1">
            <h1 class="text-xl font-bold text-gray-600">{{ time }}</h1>
          </div>
        </div>

        <!-- 右侧搜索框 -->
        <div class="w-1/2">
          <div class="bg-white/90 dark:bg-white/50 backdrop-blur-sm rounded-full shadow-lg flex items-center px-3 py-2">
            <!-- 搜索引擎选择器 -->
            <div class="relative z-9999">
          <button @click.stop="showEngines = !showEngines"
            class="flex items-center space-x-2 px-3 py-1 rounded-full dark:hover:bg-transparent hover:bg-gray-100 transition-colors">
            <component :is="currentEngine.icon()" />
            <svg class="w-4 h-4 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
            </svg>
          </button>
              <!-- 搜索引擎下拉菜单 -->
              <div v-if="showEngines" :class="[showEngines ? 'fade-in-scale' : 'fade-out-scale']"
                class="absolute top-full left-0 mt-4 bg-white dark:bg-black/60 rounded-lg shadow-lg py-2 z-9999">
                <button v-for="engine in searchEngines" :key="engine.key" @click="selectEngine(engine)"
                  class="flex items-center space-x-2 px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-900 w-full text-left">
                  <component :is="engine.icon()" class="w-5 h-5" />
                  <span class="text-gray-700 dark:text-white/90">{{ engine.label }}</span>
                </button>
              </div>
            </div>

            <!-- 搜索输入框 -->
            <input v-model="searchQuery" @keyup.enter="search" type="text" :placeholder="`在 ${currentEngine.label} 中搜索`"
              class="flex-1 px-3 py-1 outline-none bg-transparent text-sm">

            <!-- 搜索按钮 -->
            <button @click="search"
              class="bg-blue-500 text-white p-1.5 rounded-full hover:bg-blue-600 transition-colors flex items-center desktop-phone">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>

  <SettingCard v-model:show="isOpenSetting" />
  <LoginCard v-model:show="isOpenLogin" />
</template>

<script setup lang="ts">
import { h, onMounted, onUnmounted, ref, watch } from 'vue'
import AccountCard from "./AccountCard.vue";
import SettingCard from "./SettingCard.vue";
import LoginCard from "./LoginCard.vue";
import { Settings16Filled } from '@vicons/fluent'
import { AccountCircleRound } from '@vicons/material'
import { Icon } from '@vicons/utils'

// 定义搜索引擎类型
interface SearchEngine {
  key: string;
  label: string;
  icon: any;
}

// 模拟SearchEngineStore，因为找不到原始模块
const searchEngineStore = {
  currentEngineKey: ref('1'),
  initCurrentEngine: () => { },
  setCurrentEngine: (key: string) => {
    searchEngineStore.currentEngineKey.value = key;
  }
};

defineProps({
  isShowSetting: {
    type: Boolean,
    default: false
  }
})

// 为函数添加类型
function renderIcon(icon: string): () => any {
  return () => {
    return h('img', { class: 'w-5 h-5', src: icon });
  };
}

const engineLink = ['https://www.baidu.com/s?wd=', 'https://www.google.com/search?q=', 'https://www.bing.com/search?q=', 'https://duckduckgo.com/?q=']

const searchEngines: SearchEngine[] = [
  {
    key: '1',
    label: '百度',
    icon: renderIcon('https://www.baidu.com/favicon.ico'),
  },
  {
    key: '2',
    label: '谷歌',
    icon: renderIcon('https://www.google.com/favicon.ico'),
  },
  {
    key: '3',
    label: '必应',
    icon: renderIcon('https://www.bing.com/favicon.ico'),
  },
  {
    key: '4',
    label: 'DuckDuckGo',
    icon: renderIcon('https://duckduckgo.com/favicon.ico'),
  }
]

const searchQuery = ref('')
const showEngines = ref(false)
const firstSticky = ref(true)
const time = ref('')
const suggestions = ref<string[]>([])
const selectedSuggestionIndex = ref(-1)
const isSticky = ref(false);
const originalNav = ref<HTMLElement | null>(null);
const currentEngine = ref<SearchEngine>(searchEngines[0])
const isOpenSuggestions = ref(false)
const isOpenSetting = ref(false)
const isOpenAccount = ref(false)
const isOpenLogin = ref(false)

// Watch for changes in the store
watch(() => searchEngineStore.currentEngineKey.value, (newKey: string) => {
  const engine = searchEngines.find(item => item.key === newKey);
  if (engine) {
    currentEngine.value = engine;
  }
});

const updateTime = function (): void {
  const now = new Date();
  // 获取小时和分钟
  const hours = now.getHours().toString().padStart(2, '0');
  const minutes = now.getMinutes().toString().padStart(2, '0');
  // 格式化时间为 'hh:mm'
  time.value = `${hours}:${minutes}`;
};

// Initialize current engine from store
const initializeCurrentEngine = (): void => {
  searchEngineStore.initCurrentEngine();
  const engine = searchEngines.find(item => item.key === searchEngineStore.currentEngineKey.value);
  if (engine) {
    currentEngine.value = engine;
  }
};

// 为参数添加类型
const selectEngine = (engine: string | SearchEngine): void => {
  // 处理直接传入key或完整对象的情况
  const engineKey = typeof engine === 'string' ? engine : engine.key;
  const foundEngine = searchEngines.find(item => item.key === engineKey);
  if (foundEngine) {
    currentEngine.value = foundEngine;
  }
  showEngines.value = false;

  // Use store instead of LocalStorageHelper
  searchEngineStore.setCurrentEngine(engineKey);
};

const search = async (): Promise<void> => {
  if (selectedSuggestionIndex.value !== -1 && selectedSuggestionIndex.value < suggestions.value.length) {
    const engineIndex = parseInt(currentEngine.value.key) - 1;
    if (engineIndex >= 0 && engineIndex < engineLink.length) {
      const searchUrl = engineLink[engineIndex] + encodeURIComponent(suggestions.value[selectedSuggestionIndex.value]);
      window.open(searchUrl);
    }
    searchQuery.value = '';
    selectedSuggestionIndex.value = -1;
    return;
  }

  if (searchQuery.value.trim()) {
    const engineIndex = parseInt(currentEngine.value.key) - 1;
    if (engineIndex >= 0 && engineIndex < engineLink.length) {
      const searchUrl = engineLink[engineIndex] + encodeURIComponent(searchQuery.value);
      window.open(searchUrl);
    }
  }
};

const transform = (): void => {
  if (searchQuery.value.trim()) {
    window.open(`https://translate.google.com/?sl=auto&tl=en&text=${encodeURIComponent(searchQuery.value)}&op=translate`);
  }
};

// 定义百度建议接口的响应类型
interface BaiduSuggestionResponse {
  s: string[];
}

const getSuggestions = async (): Promise<void> => {
  selectedSuggestionIndex.value = -1;

  if (searchQuery.value === '') {
    isOpenSuggestions.value = false;
    suggestions.value = [];
    return;
  }

  const result = await new Promise<BaiduSuggestionResponse>((resolve) => {
    const callbackName = `jsonp_${Date.now()}_${Math.floor(Math.random() * 100000)}`;
    (window as any)[callbackName] = (data: BaiduSuggestionResponse) => {
      resolve(data);
      delete (window as any)[callbackName];
      document.body.removeChild(script);
    };
    const script = document.createElement('script');
    script.src = `https://suggestion.baidu.com/su?wd=${encodeURIComponent(searchQuery.value)}&cb=${callbackName}`;
    document.body.appendChild(script);
  });

  suggestions.value = result.s || [];
  isOpenSuggestions.value = suggestions.value.length > 0;
};

const openUrl = (text: string): void => {
  const engineIndex = parseInt(currentEngine.value.key) - 1;
  if (engineIndex >= 0 && engineIndex < engineLink.length) {
    const searchUrl = engineLink[engineIndex] + encodeURIComponent(text);
    window.open(searchUrl);
  }
};

// 防抖函数，避免滚动时频繁触发
let scrollTimeout: number | undefined;
const handleScroll = (): void => {
  // 清除之前的定时器
  if (scrollTimeout) {
    clearTimeout(scrollTimeout);
  }

  // 设置新的定时器，延迟执行实际逻辑
  scrollTimeout = window.setTimeout(() => {
    if (!originalNav.value) return;

    const rect = originalNav.value.getBoundingClientRect();
    // 当原始导航完全离开视口顶部时，激活固定导航
    isSticky.value = rect.bottom <= 0;

    // 当第一次变为粘性状态时，标记firstSticky为false以显示动画
    if (isSticky.value && firstSticky.value) {
      firstSticky.value = false;
    }
  }, 50); // 50ms的防抖延迟
};

const hide = (): boolean => {
  if (showEngines.value) {
    showEngines.value = false;
    return true;
  }
  if (isOpenSuggestions.value) {
    isOpenSuggestions.value = false;
    return true;
  }
  return false;
};

// 点击外部区域关闭菜单
const handleClickOutside = (event: MouseEvent): void => {
  const target = event.target as HTMLElement;
  const chooseEngine = document.getElementById('chooseEngine');
  const suggestionsElement = target.closest('.absolute.left-0.mt-4');
  const engineDropdown = target.closest('.absolute.top-full.left-0.mt-4');
  
  // 检查是否点击了搜索引擎选择器及其内部元素，或者下拉菜单本身
  const isClickingEngine = chooseEngine?.contains(target) || engineDropdown;
  // 检查是否点击了搜索建议框
  const isClickingSuggestions = suggestionsElement;

  // 如果既不是点击搜索引擎选择器及其内部元素，也不是点击搜索建议框，则关闭菜单
  if (!isClickingEngine && !isClickingSuggestions) {
    if (showEngines.value) {
      showEngines.value = false;
    }
    if (isOpenSuggestions.value) {
      isOpenSuggestions.value = false;
    }
  }
};

// 添加和移除滚动事件监听
onMounted(() => {
  initializeCurrentEngine();
  window.addEventListener('scroll', handleScroll);
  window.addEventListener('click', handleClickOutside);
  updateTime();
  // 每分钟更新一次时间
  setInterval(updateTime, 60000);
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
  window.removeEventListener('click', handleClickOutside);
  if (scrollTimeout) {
    clearTimeout(scrollTimeout);
  }
});

defineExpose({
  name: 'hide',
  hide
});
</script>

<style scoped>
.fade-in-scale {
  transform-origin: left top;
  animation: fade-in-scale 0.3s ease-in-out;
  opacity: 1;
}

.fade-out-scale {
  transform-origin: left top;
  animation: fade-out-scale 0.3s ease-in-out;
  opacity: 0;
}

.liquid-card-in {
  animation: fade-in-scale 0.5s ease-in-out;
  opacity: 1;
}

.liquid-card-out {
  animation: fade-out-scale 0.15s ease-in-out;
  opacity: 0;
}

@keyframes fade-in-scale {
  0% {
    opacity: 0;
    transform: scale(.5)
  }

  48% {
    opacity: 1;
    transform: scale(1.03)
  }

  to {
    opacity: 1;
    transform: scale(1)
  }
}

@keyframes fade-in {
  0% {
    opacity: 0;
    transform: scale(.5)
  }

  to {
    opacity: 1;
    transform: scale(1)
  }
}

@keyframes fade-out-scale {
  0% {
    opacity: 1;
    transform: scale(1)
  }

  to {
    opacity: 0;
    transform: scale(.5)
  }
}

.liquid-card {
  /* 玻璃效果 */
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(20px) saturate(180%);
  box-shadow: 0 8px 32px rgba(31, 38, 135, 0.08),
    inset 0 1px 2px rgba(255, 255, 255, 0.6);

  /* 悬停时的额外效果 */
  transition: all 0.3s ease;
}

/* 确保内部元素的背景适配玻璃效果 */
.liquid-card .bg-white\/40 {
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
}

.liquid-card .bg-white\/90 {
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(10px);
}

/* 搜索框的玻璃效果增强 */
.liquid-card .bg-white\/90.rounded-full {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1),
    inset 0 0 0 1px rgba(255, 255, 255, 0.5);
}
</style>