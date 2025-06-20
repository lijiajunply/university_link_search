<template>
  <div @click="hideMenu">
    <div class="home-bg" :class="[isRightMenuVisible ? 'blur-sm' : 'blur-none']"></div>

    <!-- 搜索栏 -->
    <SearchBar :is-show-setting="isRightMenuVisible" ref="searchRef"/>

    <div class="mx-auto px-8 md:px-16 pt-8 z-[-10]">
      <TilesCard class="opacity-0 animate-fade-in"
                 :style="{ animationDelay: `${0.1}s`}"/>
    </div>

    <template v-if="categories.length > 0">
      <!-- 网站分类卡片 -->
      <div class="mx-auto px-8 md:px-16 pt-8 z-[-10]">
        <div class="flex justify-center items-center">
          <div :class="[
                'grid grid-cols-8 gap-6 md:gap-8']">
            <div
                v-for="(category, index) in categories"
                :key="index"
                :class="getCategoryColClass(index)"
                class="opacity-0 animate-fade-in"
                :style="{ animationDelay: `${index * 0.2}s`}"
            >
              <AppleCard class="h-full">
                <template #title>
                  <div class="flex gap-2.5">
                    <IconFont
                        :type="`#icon-${category.icon}`"
                        class="text-[40px]"
                    />
                    <div class="font-bold text-[30px] leading-[1.35em] dark:text-white/90">
                      {{ category.name }}
                    </div>
                  </div>
                  <div class="text-black/45 mt-0.5 mb-0.5 dark:text-white/65">
                    {{ category.description }}
                  </div>
                </template>

                <template #context>
                  <div class="grid grid-cols-12 gap-x-1 gap-y-3 mb-4 mt-2">
                    <div
                        v-for="link in category.links"
                        :class="getLinkColClass(index)"
                    >
                      <a
                          v-if="!isWeiXin && link.description === '微信打开'"
                          @click="qrCodeModalOpen(link)"
                          class="a-btn"
                          :title="link.description"
                      >
                        <div class="flex flex-col justify-center items-center">
                          <template v-if="link.icon">
                            <img
                                v-if="link.icon.startsWith('http')"
                                :src="link.icon"
                                :alt="link.name"
                                class="h-10 w-10 rounded"
                            />
                            <IconFont
                                v-else
                                :type="`#icon-${link.icon}`"
                                class="text-[40px]"
                            />
                          </template>
                          <img
                              v-else
                              :src="getIcon(link.url)"
                              :alt="link.name"
                              class="h-10 w-10 rounded"
                          />
                          <div class="btn-description text-black/97 dark:text-white/65">{{ link.name }}</div>
                        </div>
                      </a>

                      <a
                          v-else
                          :href="link.url"
                          target="_blank"
                          class="a-btn"
                          :title="link.description"
                      >
                        <div class="flex flex-col justify-center items-center">
                          <template v-if="link.icon">
                            <img
                                v-if="link.icon.startsWith('http')"
                                :src="link.icon"
                                :alt="link.name"
                                class="h-10 w-10 rounded"
                            />
                            <IconFont
                                v-else
                                :type="`#icon-${link.icon}`"
                                class="text-[40px]"
                            />
                          </template>
                          <img
                              v-else
                              :src="getIcon(link.url)"
                              :alt="link.name"
                              class="h-10 w-10 rounded"
                          />
                          <div class="btn-description text-black/97 dark:text-white/65">{{ link.name }}</div>
                        </div>
                      </a>
                    </div>
                  </div>
                </template>
              </AppleCard>
            </div>
          </div>
        </div>
      </div>

      <div class="opacity-0 animate-fade-in"
           :style="{ animationDelay: `${(categories.length+1) * 0.2}s`}">
        <FooterContent/>
      </div>
    </template>

    <QrCodeModal
        v-model:show="showQrCodeModal"
        :currentLink="currentModalLink"
    />

    <SettingCard
        v-model:show="showSetting"
    />
  </div>
</template>

<style scoped>
.home-bg {
  background-image: linear-gradient(to bottom right, #ff6b6b, #7171ae, #10bce7);
  min-height: 100vh;
  height: 100%;
  left: 0;
  -o-object-fit: cover;
  object-fit: cover;
  position: fixed;
  top: 0;
  transition: filter .25s, opacity 1s, transform .25s;
  width: 100%;
  z-index: -9999;
}

.btn-description {
  text-align: center;
  font-size: 0.8em;
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

.animate-fade-in {
  animation: fade-in-scale 0.6s ease forwards;
}
</style>

<script setup>
import AppleCard from './components/AppleCard.vue'
import IconFont from './components/IconFont.vue'
import SearchBar from "./components/SearchBar.vue";

import {onMounted, onUnmounted, ref, watch} from 'vue'
import FooterContent from "./components/FooterContent.vue";
import QrCodeModal from "./components/QrCodeModal.vue";
import SettingCard from "./components/SettingCard.vue";
import TilesCard from "./components/TilesCard.vue";

const isWeiXin = ref(false)
const categories = ref([])
const showQrCodeModal = ref(false);
const currentModalLink = ref(null);
const isRightMenuVisible = ref(false)
const searchRef = ref()
const showSetting = ref(false)

const getCategoryColClass = (index) => {
  const isLastOdd = index === categories.value.length - 1 && categories.value.length % 2 !== 0

  if (isLastOdd) {
    return 'col-span-8 md:col-span-4 lg:col-span-8'
  }

  const i = index % 4 === 1 || index % 4 === 2 ? 5 : 3

  return i === 5 ? 'col-span-8 md:col-span-4 lg:col-span-5' : "col-span-8 md:col-span-4 lg:col-span-3";
}

const getLinkColClass = (categoryIndex) => {
  const isLastOdd = categoryIndex === categories.value.length - 1 && categories.value.length % 2 !== 0

  let lgColClass

  if (isLastOdd) {
    lgColClass = 'md:col-span-2 lg:col-span-1'
  } else {
    const i = categoryIndex % 4 === 1 || categoryIndex % 4 === 2 ? 15 : 9
    const col = i === 15 ? 2 : 3

    lgColClass = col === 2
        ? 'md:col-span-4 lg:col-span-2'
        : 'md:col-span-4 lg:col-span-3'
  }

  return `col-span-4 ${lgColClass} `
}

const qrCodeModalOpen = (link) => {
  currentModalLink.value = link;
  showQrCodeModal.value = true;
}

const getIcon = (url) => {
  // 移除协议部分
  const cleanUrl = url.replace(/^https?:\/\//, '');
  // 获取第一个斜杠之前的部分（即根域名）
  const rootDomain = cleanUrl.split('/')[0];
  // 拼接形成 favicon URL
  return `https://${rootDomain}/favicon.ico`;
}

// 显示菜单
const showMenu = (event) => {
  event.preventDefault()
  isRightMenuVisible.value = true
}

// 隐藏菜单
const hideMenu = (event) => {
  const target = event.target
  if (target.id === 'setting') {
    event.preventDefault()
    console.log('setting')
    return;
  }
  if (searchRef.value.hide()) return;
  isRightMenuVisible.value = false
}

onMounted(async () => {

  const handleKeydown = (event) => {
    if (event.key === 'Escape') {
      hideMenu(event)
    }
  }

  const handleContextMenu = (event) => {
    // 检查是否在输入框等元素上右键
    const target = event.target
    if (target.tagName === 'INPUT' || target.tagName === 'TEXTAREA' || target.isContentEditable || target.id === 'chooseEngine') {
      return // 允许默认右键菜单
    }

    showMenu(event)
  }

  document.addEventListener('keydown', handleKeydown)
  document.addEventListener('contextmenu', handleContextMenu)

  onUnmounted(() => {
    document.removeEventListener('keydown', handleKeydown)
    document.removeEventListener('contextmenu', handleContextMenu)
  })

  const storageKey = 'site-theme';
  const theme = ref(localStorage.getItem(storageKey) || '');
  if (!theme.value) {
    const isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    theme.value = isDark ? 'dark' : 'light';
  }

  const apply = (val) => {
    const html = document.documentElement;
    if (val === 'dark') html.classList.add('dark');
    else html.classList.remove('dark');
    localStorage.setItem(storageKey, val);
  };

  // 监听 theme 变化
  watch(theme, (val) => {
    apply(val);
  }, {immediate: true});

  const ua = navigator.userAgent
  isWeiXin.value = !!/MicroMessenger/i.test(ua)

  categories.value = await fetch('https://link.xauat.site/api/Link/GetCategory').then(res => res.json())
})
</script>