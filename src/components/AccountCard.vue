<template>
  <div class="p-6 rounded-lg w-sm backdrop-blur-xs bg-white/50 dark:bg-black/50">
    <template v-if="isLogin">
      <div class="flex justify-center mb-1">
        <img alt="" :src="`/src/assets/${userData.Gender}生.png`" style="height: 50px;width: 50px">
      </div>
      <p class="text-gray-700 dark:text-gray-300 text-xl font-bold text-center mb-1">{{userData.UserName}}</p>

      <!-- 副标题 -->
      <p class="text-gray-500 dark:text-gray-100 text-center text-sm">
        点击即可进入账号管理
      </p>
    </template>

    <template v-else>
      <!-- 头像占位符 -->
      <div class="flex justify-center mb-1">
        <Icon class="text-gray-600 dark:text-gray-200 text-4xl">
          <AccountCircleRound/>
        </Icon>
      </div>

      <!-- 主标题 -->
      <p class="text-gray-700 dark:text-gray-300 text-xl font-bold text-center mb-1">您尚未登录</p>

      <!-- 副标题 -->
      <p class="text-gray-500 dark:text-gray-100 text-center text-sm">
        登录后，即可云端同步您的个性化设置、网站捷径和便笺
      </p>
    </template>
  </div>
</template>
<script setup lang="ts">
import {AccountCircleRound} from '@vicons/material'
import {Icon} from '@vicons/utils'
import {onMounted, ref} from 'vue'

let token = localStorage.getItem('token')
const isLogin = ref((token !== null && token.length !== 0));
const userData = ref({})

onMounted(() => {
  if (isLogin.value) {
    let userJson = localStorage.getItem('user')
    if (userJson) {
      userData.value = JSON.parse(userJson)
      console.log(userData.value)
    }
  }
})
</script>