<template>
  <div class=""
       style="background-image: linear-gradient(to bottom right, #ff6b6b, #7171ae, #10bce7); min-height: 100vh">
    <!-- 搜索栏 -->
    <SearchBar/>

    <template v-if="categories.length > 0">
      <!-- 网站分类卡片 -->
      <div class="container mx-auto px-8 md:px-16 pt-16">
        <div class="flex justify-center items-center">
          <div :class="[
                'grid grid-cols-8 gap-6 md:gap-8 transition-opacity duration-1000 ease-in-out',
                isVisible ? 'opacity-100' : 'opacity-0']">
            <div
                v-for="(category, index) in categories"
                :key="index"
                :class="getCategoryColClass(index)"
            >
              <AppleCard class="h-full">
                <template #title>
                  <div class="flex gap-2.5">
                    <IconFont
                        :type="`#icon-${category.icon}`"
                        class="text-[40px]"
                    />
                    <div class="font-bold text-[30px] leading-[1.35em]">
                      {{ category.name }}
                    </div>
                  </div>
                  <div class="text-black/45 mt-0.5 mb-0.5">
                    {{ category.description }}
                  </div>
                </template>

                <template #context>
                  <div class="grid grid-cols-12 gap-x-1 gap-y-3 mb-2 mt-2">
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
                          <div class="btn-description">{{ link.name }}</div>
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
                          <div class="btn-description">{{ link.name }}</div>
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

      <div :class="['transition-opacity duration-1000 ease-in-out',
                isVisible ? 'opacity-100' : 'opacity-0']">
        <FooterContent/>
      </div>
    </template>

    <QrCodeModal
        v-model:show="showQrCodeModal"
        :currentLink="currentModalLink"
    />
  </div>
</template>

<style scoped>


.btn-description {
  color: #00000073;
  text-align: center;
  font-size: 0.8em;
}
</style>

<script setup>
import AppleCard from './components/AppleCard.vue'
import IconFont from './components/IconFont.vue'
import SearchBar from "./components/SearchBar.vue";

import {onMounted, ref} from 'vue'
import FooterContent from "./components/FooterContent.vue";
import QrCodeModal from "./components/QrCodeModal.vue";

const isWeiXin = ref(false)
const isVisible = ref(false)
const categories = ref([])
const showQrCodeModal = ref(false);
const currentModalLink = ref(null);

onMounted(async () => {
  categories.value = await fetch('https://link.xauat.site/api/Link/GetCategory').then(res => res.json())
// 组件挂载后触发浮现效果
  setTimeout(() => {
    isVisible.value = true
  }, 100) // 短暂延迟以确保过渡效果正常

  const ua = navigator.userAgent
  isWeiXin.value = !!/MicroMessenger/i.test(ua)
})

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

  let lgColClass = ''

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
</script>