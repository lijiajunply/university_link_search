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

        <div class="mt-4 space-y-4">
          <div class="flex items-center justify-between">
            <div class="dark:text-gray-300 leading-[1.35em]">
              是否显示磁贴
            </div>
            <n-switch v-model:value="isShow" @update:value="handleChange"/>
          </div>

          <div class="flex items-center justify-between">
            <div class="dark:text-gray-300 leading-[1.35em]">
              深色模式
            </div>
            <n-switch v-model:value="isDarkMode" />
          </div>

          <div v-if="authStore.isAuthenticated" class="pt-4 border-t dark:border-gray-600">
            <n-button block type="error" @click="handleLogout">
              退出登录
            </n-button>
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import {NSwitch, NButton, useMessage} from "naive-ui";
import {computed} from "vue";
import {useSettingStore} from "../stores/SettingStore";
import {useAuthorizationStore} from "../stores/Authorization";
import {useThemeStore} from "../stores/ThemeStore";
import {storeToRefs} from "pinia";

defineProps({
  show: Boolean,
});

const emit = defineEmits(['update:show']);
const settingStore = useSettingStore();
const authStore = useAuthorizationStore();
const themeStore = useThemeStore();
const { theme } = storeToRefs(themeStore);
const message = useMessage();

function closeModal() {
  emit('update:show', false);
}

const isShow = computed({
  get: () => settingStore.showTiles,
  set: (value) => settingStore.setShowTiles(value)
})

const isDarkMode = computed({
  get: () => theme.value === 'dark',
  set: (val) => theme.value = val ? 'dark' : 'light'
})

const handleChange = (value) => {
  settingStore.setShowTiles(value);
}

const handleLogout = () => {
  authStore.logout();
  message.success("已退出登录");
  closeModal();
}
</script>

<style scoped>

</style>