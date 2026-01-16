<script setup lang="ts">
import { onMounted, ref } from "vue";
import QrCodeModal from "../components/QrCodeModal.vue";
import SettingCard from "../components/SettingCard.vue";
import TilesCard from "../components/TilesCard.vue";
import IconFont from "../components/IconFont.vue";
import AppleCard from "../components/AppleCard.vue";
import FooterContent from "../components/FooterContent.vue";
import SearchBar from "../components/SearchBar.vue";
import LinkCard from "../components/home/LinkCard.vue";
import { CategoryService } from "../services/CategoryService";
import { useThemeStore } from "../stores/ThemeStore";
import { useContextMenu } from "../composables/useContextMenu";
import { getCategoryColClass, getLinkColClass } from "../utils/layoutHelper";

const categories = ref<any[]>([])
const showQrCodeModal = ref(false);
const currentModalLink = ref(null);
const searchRef = ref()
const showSetting = ref(false)

// 使用 Composables
useThemeStore();
const { isRightMenuVisible, hideMenu } = useContextMenu(searchRef);

const qrCodeModalOpen = (link: any) => {
  currentModalLink.value = link;
  showQrCodeModal.value = true;
}

onMounted(async () => {
  categories.value = await CategoryService.getAllCategories()
})
</script>

<template>
  <div @click="hideMenu" class="relative">
    <div class="home-bg" :class="[isRightMenuVisible ? 'blur-sm' : 'blur-none']"></div>

    <!-- 搜索栏 -->
    <SearchBar :is-show-setting="isRightMenuVisible" ref="searchRef"/>

    <div class="mx-auto px-8 md:px-16 pt-8 -z-10">
      <TilesCard class="opacity-0 animate-fade-in"
                 :style="{ animationDelay: `${0.1}s`}"/>
    </div>

    <template v-if="categories.length > 0">
      <!-- 网站分类卡片 -->
      <div class="mx-auto px-8 md:px-16 pt-8 -z-10">
        <div :class="[
                'grid grid-cols-8 gap-6 md:gap-8']">
          <div
              v-for="(category, index) in categories"
              :key="index"
              :class="getCategoryColClass(index, categories.length)"
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
                  <div class="font-bold text-[30px] leading-[1.35em] dark:text-white/80">
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
                      :class="getLinkColClass(index, categories.length)"
                  >
                    <LinkCard 
                      :link="link" 
                      @open-qr="qrCodeModalOpen" 
                    />
                  </div>
                </div>
              </template>
            </AppleCard>
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

@media (prefers-color-scheme: dark) {
  .home-bg {
    background-image: linear-gradient(to top, #1e3c72 0%, #1e3c72 1%, #2a5298 100%);
  }
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
