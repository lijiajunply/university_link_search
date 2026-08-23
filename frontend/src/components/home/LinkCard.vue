<script setup lang="ts">
import { computed } from 'vue';
import { checkIsWeiXin } from '../../utils/urlHelper';
import AppLogo from '../AppLogo.vue';

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
      <AppLogo :url="link.url" :name="link.name" :icon="link.icon" />
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
