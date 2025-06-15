<template>
  <Teleport to="body">
    <div
        v-if="show"
        class="fixed inset-0 backdrop-blur-md bg-black/30 flex items-center justify-center z-50"
        @click="closeModal"
    >
      <div
          class="bg-white rounded-lg p-8 w-11/12 max-w-md mx-auto shadow-xl"
          @click.stop
      >
        <div class="text-center">
          <h2 class="text-3xl font-bold text-gray-800 mb-4">登录</h2>
          <p class="text-gray-600 text-sm mb-8">使用您的iMember账户</p>
        </div>

        <form @submit.prevent="handleSubmit">
          <!-- 姓名输入框 -->
          <div class="mb-6">
            <label for="name" class="block text-gray-700 font-medium mb-2">姓名</label>
            <input
                type="text"
                id="name"
                v-model="form.name"
                placeholder="请输入您的姓名"
                class="w-full px-4 py-3 rounded-md border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <!-- 学号输入框 -->
          <div class="mb-8">
            <label for="id" class="block text-gray-700 font-medium mb-2">学号</label>
            <input
                type="text"
                id="id"
                v-model="form.id"
                placeholder="请输入您的学号"
                class="w-full px-4 py-3 rounded-md border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <!-- 提交按钮 -->
          <div class="mt-10">
            <button
                type="submit"
                class="w-full bg-blue-500 hover:bg-blue-600 text-white font-bold py-3 px-4 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition duration-200"
            >
              提交
            </button>
          </div>
        </form>

        <!-- 注册链接 -->
        <div class="mt-8 text-center text-sm">
          <p class="text-gray-600">
            还没有账户？
            <a href="https://www.xauat.site/Signup" class="text-blue-500 hover:text-blue-700 font-medium inline-flex items-center">
              立即注册!
              <Icon class="ml-1 ">
                <LinkRound/>
              </Icon>
            </a>
          </p>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import '../lib/gzip.js'
import {LinkRound} from '@vicons/material'
import {Icon} from '@vicons/utils'

defineProps({
  show: Boolean,
});

const emit = defineEmits(['update:show']);

function closeModal() {
  emit('update:show', false);
}

import {reactive} from 'vue'
import {unzipObj} from "../lib/gzip.js";

const form = reactive({
  name: '',
  id: ''
})

const handleSubmit = async () => {
  // 这里处理表单提交逻辑
  console.log('提交表单数据:', form)
  // 可以在这里添加表单验证和发送请求的逻辑
  let res = await fetch('https://ioslife.zeabur.app/User', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(form)
  })
  if (res.ok) {
    const token = await res.text()
    localStorage.setItem('token', token)
    res = await fetch('https://ioslife.zeabur.app/User', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      }
    })
    if (res.ok) {
      const user = unzipObj(await res.text())
      localStorage.setItem('user', JSON.stringify(user))
    }
    closeModal()
  } else {
    alert('登录失败')
  }
}
</script>

<style scoped>

</style>