<template>
  <div class="flex items-center justify-center p-4">
    <div class="w-full max-w-md space-y-8">
      <!-- Logo和标题 -->
      <div class="text-center space-y-2">
        <div
          class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-primary/10 text-primary dark:bg-primary/20">
          <Icon icon="ep:link" class="w-8 h-8" />
        </div>
        <h1 class="text-3xl font-semibold text-gray-900 dark:text-white">建大百科登录</h1>
        <p class="text-gray-600 dark:text-gray-300">登录您的 iMember 账户</p>
      </div>

      <!-- OAuth登录按钮 -->
      <div>
        <button type="button" @click="handleOAuthLogin"
          class="group relative w-full flex justify-center py-3 px-4 border border-gray-300 dark:border-gray-600 rounded-xl text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-850 hover:bg-gray-50 dark:hover:bg-gray-800 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all duration-200">
          <span class="absolute left-0 inset-y-0 flex items-center pl-3">
            <Icon icon="ep:link" class="h-5 w-5 text-primary" />
          </span>
          <span>使用 iMember 账户登录</span>
        </button>
      </div>

      <!-- 错误信息 -->
      <n-alert v-if="errorMessage" :title="'登录失败'" :description="errorMessage" type="error" show-icon
        class="rounded-xl" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useMessage } from 'naive-ui'
import { Icon } from '@iconify/vue'

// 路由
const router = useRouter()
const message = useMessage()

// 表单数据
const form = reactive({
  username: '',
  password: '',
  rememberMe: false
})

// 登录加载状态
const loading = ref(false)

// 错误信息
const errorMessage = ref('')

// 处理OAuth登录跳转
const handleOAuthLogin = () => {
  // 生成state参数并保存，用于验证回调
  const state = generateState()
  localStorage.setItem('oauth_state', state)

  // 构建重定向URL
  const currentUrl = window.location.origin + window.location.pathname
  const redirectUri = encodeURIComponent(currentUrl)

  // 跳转到后端的authorize端点
  window.location.href = `/api/auth/authorize?redirectUri=${redirectUri}`
}

// 生成随机的state参数
const generateState = () => {
  return Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15)
}

// 处理OAuth回调
const handleOAuthCallback = async () => {
  // 解析URL参数
  const urlParams = new URLSearchParams(window.location.search)
  const code = urlParams.get('code')
  const state = urlParams.get('state')

  // 检查是否有code和state参数
  if (code && state) {
    // 验证state参数
    const storedState = localStorage.getItem('oauth_state')
    if (storedState !== state) {
      message.error('无效的请求，请重试')
      localStorage.removeItem('oauth_state')
      return
    }

    loading.value = true
    errorMessage.value = ''

    try {
      // 调用后端回调接口获取token
      const response = await fetch(`/api/auth/callback?code=${code}&state=${state}`)
      const data = await response.json()

      if (!response.ok) {
        throw new Error(data.message || '登录失败')
      }

      // 存储token
      if (data.accessToken) {
        localStorage.setItem('auth_token', data.accessToken)
      }

      // 清除临时state
      localStorage.removeItem('oauth_state')

      // 登录成功，跳转到首页
      message.success('登录成功')
      router.push('/')
    } catch (error) {
      errorMessage.value = error instanceof Error ? error.message : 'OAuth登录失败，请稍后重试'
      message.error(errorMessage.value)
      localStorage.removeItem('oauth_state')
    } finally {
      loading.value = false
    }
  }
}

// 组件挂载时执行
onMounted(() => {
  // 首先检查是否是OAuth回调
  handleOAuthCallback()

  // 如果有记住的用户名则填充
  const rememberedUsername = localStorage.getItem('rememberedUsername')
  if (rememberedUsername) {
    form.username = rememberedUsername
    form.rememberMe = true
  }
})
</script>

<style scoped>
/* 自定义输入框动画 */
input:focus {
  transform: translateY(-1px);
}

/* 按钮悬停效果 */
button:enabled:hover {
  transform: translateY(-1px);
}

/* 平滑过渡 */
* {
  transition-property: background-color, border-color, color, fill, stroke, opacity, box-shadow, transform;
}
</style>