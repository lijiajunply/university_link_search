<template>
  <!-- 原始组件 -->
  <div ref="originalNav" class="pt-12 pb-8">
    <div class="container mx-auto">
      <div class="grid grid-cols-3 gap-4 mt-8">
        <div></div>
        <div class="flex justify-center">
          <div class="bg-white/10 backdrop-blur-sm rounded-lg px-4 py-2 space-x-2">
            <h1 class="text-4xl font-bold text-white">
              {{ time }}
            </h1>
          </div>
        </div>
        <div>
          <div class="flex justify-end" v-if="isShowSetting">
            <div class="relative">
              <div
                  @mouseenter="isOpenAccount = true"
                  @mouseleave="isOpenAccount = false"
                  class="flex items-center space-x-2 mx-2 p-2 rounded-full hover:bg-gray-100 transition-colors"
              >
                <AccountCircleOutlineIcon class="text-gray-600"/>
              </div>
              <div
                  class="absolute top-full right-1 mt-2 py-2 z-50"
                  v-show="isOpenAccount"
                  @mouseenter="isOpenAccount = true"
                  @mouseleave="isOpenAccount = false"
              >
                <AccountCard/>
              </div>
            </div>
            <div
                @click="isOpenSetting = !isOpenSetting"
                id="setting"
                class="flex items-center space-x-2 mx-2 p-2 rounded-full hover:bg-gray-100 transition-colors"
            >
              <CogIcon class="text-gray-600"/>
            </div>
          </div>
        </div>
      </div>

      <div class="mx-8 md:mx-auto mt-8 md:max-w-2xl">
        <div class="bg-white/90 backdrop-blur-sm rounded-full shadow-lg flex items-center px-4 py-3">
          <!-- 搜索引擎选择器 -->
          <div class="relative" id="chooseEngine">
            <button
                @click="engineClick"
                class="flex items-center space-x-2 px-3 py-1 rounded-full hover:bg-gray-100 transition-colors"
            >
              <img :src="currentEngine.icon" :alt="currentEngine.name" class="w-5 h-5">
              <svg class="w-4 h-4 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
              </svg>
            </button>

            <!-- 搜索引擎下拉菜单 -->
            <div :class="['absolute top-full left-0 mt-4 bg-white rounded-lg shadow-lg py-2 z-50',
            firstShowEngines ? 'opacity-0' : showEngines ? 'fade-in-scale' : 'fade-out-scale']">
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
          <div class="relative flex-1 px-4 py-1">
            <input
                v-model="searchQuery"
                @keyup.enter="search"
                @keyup="getSuggestions"
                type="text"
                :placeholder="`在 ${currentEngine.name} 中搜索`"
                class="outline-none bg-transparent"
            >
            <div class="absolute left-0 mt-4 bg-white/70 backdrop-blur-sm rounded-lg shadow-lg z-40 "
                 v-if="isOpenSuggestions">
              <button
                  @click="transform"
                  class="flex items-center space-x-2 px-4 py-2 hover:bg-gray-100 w-full text-left rounded-lg"
              >
                <span class="text-gray-700 text-sm">翻译: {{ searchQuery }}</span>
              </button>
              <button
                  v-for="(suggestion, index) in suggestions"
                  @click="openUrl(suggestion)"
                  :class="{ 'bg-gray-100': index === selectedSuggestionIndex }"
                  class="flex items-center space-x-2 px-4 py-2 hover:bg-gray-100 w-full text-left rounded-lg"
              >
                <span class="text-gray-700 text-sm">{{ suggestion }}</span>
              </button>
            </div>
          </div>

          <!-- 搜索按钮 -->
          <button
              @click="search"
              class="bg-blue-500 text-white p-2 rounded-full hover:bg-blue-600 transition-colors flex items-center space-x-2 desktop-phone"
          >
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
  <div v-if="isSticky"
       class="fixed top-0 left-0 right-0 py-2 z-50 transition-all duration-300 transform translate-y-0 mx-8 my-4 rounded-lg liquid-card">
    <div class="container mx-auto px-4">
      <div class="flex items-center justify-between">
        <!-- 左侧时间显示 -->
        <div class="flex items-center">
          <div class="bg-white/40 backdrop-blur-sm rounded-lg px-3 py-1">
            <h1 class="text-xl font-bold text-gray-600">{{ time }}</h1>
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
                class="bg-blue-500 text-white p-1.5 rounded-full hover:bg-blue-600 transition-colors flex items-center desktop-phone"
            >
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

  <svg style="position:absolute;width:0;height:0" v-if="isSticky">
    <filter id="frosted" primitiveUnits="objectBoundingBox">
      <feImage
          href="licard.png"
          x="0" y="0" width="100" height="100" result="map"/>
      <feGaussianBlur in="SourceGraphic" stdDeviation="0.02" result="blur"/>
      <feDisplacementMap id="disp" in="blur" in2="map" scale="1" xChannelSelector="R" yChannelSelector="G"/>
    </filter>
  </svg>
</template>

<script setup>
import {onMounted, onUnmounted, ref} from 'vue'
import CogIcon from "vue-material-design-icons/Cog.vue";
import AccountCircleOutlineIcon from "vue-material-design-icons/AccountCircleOutline.vue";
import AccountCard from "./AccountCard.vue";
import SettingCard from "./SettingCard.vue";

defineProps({
  isShowSetting: {
    type: Boolean,
    default: false
  }
})

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

const searchQuery = ref('')
const showEngines = ref(false)
const firstShowEngines = ref(true)
const time = ref('')
const suggestions = ref([])
const selectedSuggestionIndex = ref(-1)
const isSticky = ref(false);
const originalNav = ref(null);
const currentEngine = ref(searchEngines[0])
const isOpenSuggestions = ref(false)
const isOpenSetting = ref(false)
const isOpenAccount = ref(false)

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

const engineClick = () => {
  showEngines.value = !showEngines.value;
  firstShowEngines.value = false;
}

const search = async () => {
  if (selectedSuggestionIndex.value !== -1) {
    window.open(currentEngine.value.searchUrl + encodeURIComponent(suggestions.value[selectedSuggestionIndex.value]), '_blank')
    searchQuery.value = ''
    selectedSuggestionIndex.value = -1
    return
  }

  if (searchQuery.value.trim()) {
    window.open(currentEngine.value.searchUrl + encodeURIComponent(searchQuery.value), '_blank')
  }
}

const transform = () => {
  if (searchQuery.value.trim()) {
    window.open(`https://translate.google.com/?sl=auto&tl=en&text=${encodeURIComponent(searchQuery.value)}&op=translate`, '_blank')
  }
}

const getSuggestions = async () => {
  selectedSuggestionIndex.value = -1

  if (searchQuery.value === '') {
    isOpenSuggestions.value = false
    suggestions.value = []
    return
  }
  const result = await new Promise((resolve) => {
    const callbackName = `jsonp_${Date.now()}_${Math.floor(Math.random() * 100000)}`;
    window[callbackName] = (data) => {
      resolve(data);
      delete window[callbackName];
      document.body.removeChild(script);
    };
    const script = document.createElement('script');
    script.src = `https://suggestion.baidu.com/su?wd=${encodeURIComponent(searchQuery.value)}&cb=${callbackName}`;
    document.body.appendChild(script);
  });
  suggestions.value = result.s
  isOpenSuggestions.value = suggestions.value.length > 0
}

const openUrl = (text) => {
  window.open(currentEngine.value.searchUrl + encodeURIComponent(text), '_blank')
}

// 滚动监听函数
const handleScroll = () => {
  if (!originalNav.value) return;

  const rect = originalNav.value.getBoundingClientRect();
  // 当原始导航完全离开视口顶部时，激活固定导航
  isSticky.value = rect.bottom <= 0;
};

const hide = () => {
  if (isOpenSetting.value) {
    console.log(isOpenSetting.value)
    isOpenSetting.value = false
    return true
  }

  if (showEngines.value) {
    console.log(showEngines.value)
    return true
  }
  if (isOpenSuggestions.value) {
    isOpenSuggestions.value = false
    console.log(isOpenSuggestions.value)
    return true
  }
  return false
}

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

defineExpose({
  name: 'hide',
  hide
})
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
  box-shadow:
      0 8px 32px rgba(31, 38, 135, 0.08),
      inset 0 1px 2px rgba(255, 255, 255, 0.6);

  /* 悬停时的额外效果 */
  transition: all 0.3s ease;
  animation: fade-in-scale 0.5s ease-in-out;
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