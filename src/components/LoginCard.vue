<template>
  <Teleport to="body">
    <div
        v-if="show"
        class="fixed inset-0 backdrop-blur-md bg-black/30 flex items-center justify-center z-50"
        @click="closeModal"
    >
      <div
          class="bg-white/80 dark:bg-gray-800/70 rounded-lg p-8 w-11/12 max-w-md mx-auto shadow-xl"
          @click.stop
      >
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
                  <n-input
                      type="password"
                      show-password-on="mousedown"
                      placeholder="密码"
                      autosize style="min-width: 50%"
                      class=""
                      v-model:value="eduPassword"
                  />
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
            <p class="text-gray-600 dark:text-gray-200 text-sm mb-8">使用您的iMember账户</p>
          </div>

          <form @submit.prevent="handleSubmit">
            <!-- 姓名输入框 -->
            <div class="mb-6">
              <label for="name" class="block text-gray-700 dark:text-gray-300 font-medium mb-2">姓名</label>
              <input
                  type="text"
                  id="name"
                  v-model="form.name"
                  placeholder="请输入您的姓名"
                  class="w-full px-4 py-3 rounded-md border border-gray-300 dark:border-transparent dark:text-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <!-- 学号输入框 -->
            <div class="mb-8">
              <label for="id" class="block text-gray-700 dark:text-gray-200 font-medium mb-2">学号</label>
              <input
                  type="text"
                  id="id"
                  v-model="form.id"
                  placeholder="请输入您的学号"
                  class="w-full px-4 py-3 rounded-md border border-gray-300 dark:border-transparent dark:text-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <!-- 提交按钮 -->
            <div class="mt-10">
              <button
                  type="submit"
                  class="w-full bg-blue-500 hover:bg-blue-600 text-white dark:text-gray-300 font-bold py-3 px-4 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition duration-200"
              >
                提交
              </button>
            </div>
          </form>

          <!-- 注册链接 -->
          <div class="mt-8 text-center text-sm">
            <p class="text-gray-600">
              还没有账户？
              <a href="https://www.xauat.site/Signup"
                 class="text-blue-500 hover:text-blue-700 font-medium inline-flex items-center">
                立即注册!
                <Icon class="ml-1 ">
                  <LinkRound/>
                </Icon>
              </a>
            </p>
          </div>
        </template>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import {LinkRound} from '@vicons/material'
import {Icon} from '@vicons/utils'
import {reactive, ref} from 'vue'
import {NSwitch, NInput, NButton} from "naive-ui";
import {eduLoginService} from '/src/services/EduLoginService.js'
import { useAuthorizationStore } from '../stores/Login'
import { LoginService } from '../services/LoginService'

defineProps({
  show: Boolean,
});

const store = useAuthorizationStore();
const isLogin = ref(store.isAuthenticated);

const isShow = ref(localStorage.getItem('is-show-edu') === 'true')
const userData = ref(null)
const eduPassword = ref('')
eduPassword.value = localStorage.getItem('edu-password') ?? ''
let userJson = localStorage.getItem('user')
if (userJson) {
  userData.value = JSON.parse(userJson)
}

const handleChange = (value) => {
  localStorage.setItem('is-show-edu', value)
}

const handleGetData = async () => {
  await eduLoginService(userData.value.UserId, eduPassword.value)
}

// 登录时

const emit = defineEmits(['update:show']);

function closeModal() {
  emit('update:show', false);
}

const form = reactive({
  name: '',
  id: ''
})

const handleSubmit = async () => {
  // 这里处理表单提交逻辑
  console.log('提交表单数据:', form)
  // 可以在这里添加表单验证和发送请求的逻辑
  try {
    const loginResult = await LoginService.login(form.name, form.id);
    if (loginResult) {
      // 登录成功，更新 store
      await store.login({ username: form.name, password: form.id });
      closeModal();
    } else {
      alert('登录失败');
    }
  } catch (error) {
    alert('登录失败: ' + error.message);
  }
}
</script>

<style scoped>

</style>