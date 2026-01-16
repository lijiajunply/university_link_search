<script setup lang="ts">
import { computed } from 'vue';
import IconFont from '../IconFont.vue';
import { getIcon, checkIsWeiXin } from '../../utils/urlHelper';

interface Link {
  url: string;
  description: string;
  name: string;
  icon: string;
}

const props = defineProps<{
  link: Link;
}>();

const emit = defineEmits<{
  (e: 'open-qr', link: Link): void
}>();

const isWeiXin = checkIsWeiXin();

const isQrMode = computed(() => {
  return !isWeiXin && props.link.description === '微信打开';
});

const handleClick = (e: MouseEvent) => {
  if (isQrMode.value) {
    e.preventDefault();
    emit('open-qr', props.link);
  }
};
</script>

<template>
  <a
      :href="isQrMode ? undefined : link.url"
      :target="isQrMode ? undefined : '_blank'"
      class="a-btn cursor-pointer"
      :title="link.description"
      @click="handleClick"
  >
    <div class="flex flex-col justify-center items-center">
      <template v-if="link.icon">
        <img
            v-if="link.icon.startsWith('http')"
            v-lazy="link.icon"
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
          v-lazy="getIcon(link.url)"
          :alt="link.name"
          class="h-10 w-10 rounded"
      />
      <div class="btn-description text-black/97 dark:text-white/65">{{ link.name }}</div>
    </div>
  </a>
</template>

<style scoped>
.btn-description {
  text-align: center;
  font-size: 0.8em;
}
</style>
