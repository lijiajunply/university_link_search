<template>
  <Teleport to="body">
    <div
        v-if="show"
        class="fixed inset-0 backdrop-blur-md bg-black/30 flex items-center justify-center z-50"
        @click="closeModal"
    >
      <div
          class="bg-white/80 dark:bg-gray-800/70 rounded-lg p-6 w-11/12 max-w-md mx-auto shadow-xl"
          @click.stop
      >
        <div class="dark:text-gray-300 font-bold text-[30px] leading-[1.35em]">
          应用设置
        </div>

        <div class="mt-4">
          <div class="flex items-center justify-between">
            <div class="dark:text-gray-300 leading-[1.35em]">
              是否显示磁贴
            </div>
            <n-switch v-model:value="isShow" @update:value="handleChange"/>
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import {NSwitch} from "naive-ui";
import {ref} from "vue";

defineProps({
  show: Boolean,
});

const emit = defineEmits(['update:show']);

function closeModal() {
  emit('update:show', false);
}

const isShow = ref(localStorage.getItem('is-show-tiles') === 'true')

const handleChange = (value) => {
  localStorage.setItem('is-show-tiles', value)
}
</script>

<style scoped>

</style>