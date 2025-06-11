<template>
  <Teleport to="body">
    <div
        v-if="show"
        class="fixed inset-0 backdrop-blur-md bg-black/30 flex items-center justify-center z-50"
        @click="closeModal"
    >
      <div
          class="bg-white rounded-lg p-6 w-11/12 max-w-md mx-auto shadow-xl"
          @click.stop
      >
        <!-- 标题区域 -->
        <div class="flex items-center mb-5">
          <IconFont
              :type="`#icon-${currentLink.icon}`"
              class="text-[40px]"
          />
          <h2 class="text-2xl font-bold">{{ currentLink?.name || '一卡通' }}</h2>
        </div>

        <!-- 内容区域 -->
        <div class="text-center">
          <p class="mb-4 text-lg">请使用微信扫码打开</p>

          <!-- 二维码图片 -->
          <div class="flex justify-center mb-6">
            <img :src="qrCodeUrl" alt="QR Code" class="w-64 h-64"/>
          </div>

          <!-- 按钮 -->
          <button
              class="bg-blue-500 hover:bg-blue-600 text-white font-medium py-2 px-8 rounded-md transition-colors"
              @click="closeModal"
          >
            OK
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import {ref, watch} from 'vue';
import QRCode from 'qrcode';
import IconFont from "./IconFont.vue";

const props = defineProps({
  show: Boolean,
  currentLink: Object
});

const emit = defineEmits(['update:show']);

const qrCodeUrl = ref('');

// 当链接变化时生成新的二维码
watch(() => props.currentLink, async (newLink) => {
  if (newLink?.url) {
    try {
      qrCodeUrl.value = await QRCode.toDataURL(newLink.url);
    } catch (err) {
      console.error('QR Code generation error:', err);
    }
  }
}, {immediate: true});

function closeModal() {
  emit('update:show', false);
}
</script>