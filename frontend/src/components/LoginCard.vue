<template>
  <Teleport to="body">
    <div v-if="show" class="fixed inset-0 backdrop-blur-md bg-black/30 flex items-center justify-center z-50"
         @click="closeModal">
      <div class="bg-white/80 dark:bg-gray-800/70 rounded-lg p-8 w-11/12 max-w-md mx-auto shadow-xl" @click.stop>
        <template v-if="isLogin">
          <div class="text-center">
            <div class="dark:text-gray-300 font-bold text-[30px] leading-[1.35em]">
              账号设置
            </div>

            <div class="mt-4">
              <div class="flex items-center justify-between">
                <div class="dark:text-gray-300 leading-[1.35em]">
                  是否显示教务数据
                </div>
                <n-switch v-model:value="isShow" @update:value="handleChange"/>
              </div>
            </div>

            <template v-if="isShow">
              <div class="mt-4">
                <div class="flex items-center justify-between">
                  <div class="dark:text-gray-300 leading-[1.35em]">
                    教务系统密码
                  </div>
                  <n-input type="password" show-password-on="mousedown" placeholder="密码" autosize
                           style="min-width: 50%"
                           class="" v-model:value="eduPassword"/>
                </div>
              </div>
              <div class="mt-4">
                <div class="flex items-center justify-between">
                  <div class="dark:text-gray-300 leading-[1.35em]">

                  </div>
                  <n-button round type="success" @click="handleGetData">
                    获取相关数据
                  </n-button>
                </div>
              </div>
            </template>
          </div>
        </template>

        <template v-else>
          <div class="text-center">
            <h2 class="text-3xl font-bold text-gray-800 dark:text-gray-300 mb-4">登录</h2>
            <p class="text-gray-600 dark:text-gray-200 text-sm mb-8">使用您的iMember账户进行授权登录</p>
          </div>

          <!-- 跳转到授权页面的按钮 -->
          <div class="mt-6">
            <button type="button"
                    class="w-full bg-blue-500 hover:bg-blue-600 text-white dark:text-gray-300 font-bold py-3 px-4 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition duration-200"
                    @click="handleOAuthLogin">
              授权登录
            </button>
          </div>

          <!-- 授权说明 -->
          <div class="mt-4 text-center text-sm text-gray-500 dark:text-gray-400">
            <p>点击授权登录后，将跳转到统一身份认证系统进行授权</p>
          </div>
        </template>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import {ref} from 'vue'
import {NButton, NSwitch} from "naive-ui";
import {useAuthorizationStore} from '../stores/Authorization.ts'

defineProps({
  show: Boolean,
});

const store = useAuthorizationStore();
const isLogin = ref(store.isAuthenticated);

// 使用store中的getter获取状态
const isShow = ref(store.getIsShowEdu);
const userData = ref(store.getUserData);
const eduPassword = ref(store.getEduPassword);

const handleChange = (value) => {
  // 使用store中的action来更新状态
  store.setIsShowEdu(value);
  isShow.value = value;
}

const handleGetData = async () => {
  // 先更新store中的密码
  store.setEduPassword(eduPassword.value);
  // 使用store中的action来获取教育数据
  await store.handleGetEduData();
}

// 登录时

const emit = defineEmits(['update:show']);

function closeModal() {
  emit('update:show', false);
}

// 处理OAuth授权登录
const handleOAuthLogin = () => {
  window.location.href = 'https://link.xauat.site/api/auth/authorize';
}
</script>

<style scoped></style>