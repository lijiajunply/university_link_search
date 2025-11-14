<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100 dark:from-gray-900 dark:to-gray-800 p-4">
    <div class="w-full max-w-md space-y-8">
      <!-- Logo和标题 -->
      <div class="text-center space-y-2">
        <div class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-primary/10 text-primary dark:bg-primary/20">
          <Icon icon="ep:link" class="w-8 h-8" />
        </div>
        <h1 class="text-3xl font-semibold text-gray-900 dark:text-white">大学链接搜索</h1>
        <p class="text-gray-600 dark:text-gray-300">登录您的账户</p>
      </div>

      <!-- 登录表单 -->
      <div class="bg-white dark:bg-gray-800 rounded-2xl shadow-sm border border-gray-200 dark:border-gray-700 p-8 transition-all duration-200 hover:shadow-md">
        <form @submit.prevent="handleLogin" class="space-y-6">
          <div class="space-y-2">
            <label for="username" class="block text-sm font-medium text-gray-700 dark:text-gray-200">用户名</label>
            <div class="relative">
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <Icon icon="ep:user" class="h-5 w-5 text-gray-400" />
              </div>
              <input
                id="username"
                v-model="form.username"
                type="text"
                required
                class="pl-10 block w-full rounded-xl border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-3 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
                placeholder="请输入用户名"
              />
            </div>
          </div>

          <div class="space-y-2">
            <label for="password" class="block text-sm font-medium text-gray-700 dark:text-gray-200">密码</label>
            <div class="relative">
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <Icon icon="ep:lock" class="h-5 w-5 text-gray-400" />
              </div>
              <input
                id="password"
                v-model="form.password"
                :type="showPassword ? 'text' : 'password'"
                required
                class="pl-10 block w-full rounded-xl border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-3 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
                placeholder="请输入密码"
              />
              <button
                type="button"
                @click="showPassword = !showPassword"
                class="absolute inset-y-0 right-0 pr-3 flex items-center"
              >
                <Icon :icon="showPassword ? 'ep:eye' : 'ep:eye-close'" class="h-5 w-5 text-gray-400" />
              </button>
            </div>
          </div>

          <div class="flex items-center justify-between">
            <div class="flex items-center">
              <input
                id="remember-me"
                v-model="form.rememberMe"
                type="checkbox"
                class="h-4 w-4 text-primary border-gray-300 rounded focus:ring-primary/50"
              />
              <label for="remember-me" class="ml-2 block text-sm text-gray-700 dark:text-gray-300">记住我</label>
            </div>
            <a href="#" class="text-sm font-medium text-primary hover:text-primary/80 transition-colors">忘记密码？</a>
          </div>

          <div>
            <button
              type="submit"
              :disabled="loading"
              class="group relative w-full flex justify-center py-3 px-4 border border-transparent rounded-xl text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all duration-200 disabled:opacity-70 disabled:cursor-not-allowed"
            >
              <span class="absolute left-0 inset-y-0 flex items-center pl-3">
                <Icon v-if="!loading" icon="ep:arrow-right" class="h-5 w-5 text-white" />
                <n-spin v-else :size="16" stroke-width="2" class="text-white" />
              </span>
              {{ loading ? '登录中...' : '登录' }}
            </button>
          </div>

          <!-- 分隔线 -->
          <div class="relative flex items-center justify-center my-8">
            <div class="absolute inset-0 flex items-center">
              <div class="w-full border-t border-gray-300 dark:border-gray-600"></div>
            </div>
            <div class="relative px-4 bg-white dark:bg-gray-800 text-sm text-gray-500 dark:text-gray-400">
              或通过以下方式登录
            </div>
          </div>

          <!-- OAuth登录按钮 -->
          <div>
            <button
              type="button"
              @click="handleOAuthLogin"
              class="group relative w-full flex justify-center py-3 px-4 border border-gray-300 dark:border-gray-600 rounded-xl text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-850 hover:bg-gray-50 dark:hover:bg-gray-800 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all duration-200"
            >
              <span class="absolute left-0 inset-y-0 flex items-center pl-3">
                <Icon icon="ep:link" class="h-5 w-5 text-primary" />
              </span>
              <span>使用大学账户登录</span>
            </button>
          </div>
        </form>
      </div>

      <!-- 错误信息 -->
      <n-alert
        v-if="errorMessage"
        :title="'登录失败'"
        :description="errorMessage"
        type="error"
        show-icon
        class="rounded-xl"
      />

      <!-- 页脚 -->
      <div class="text-center text-sm text-gray-500 dark:text-gray-400">
        <p>© {{ new Date().getFullYear() }} 大学链接搜索系统</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { NForm } from 'naive-ui'
import { useMessage } from 'naive-ui'
import { Icon } from '@iconify/vue'
import { AuthService } from '../services/AuthService'

// 路由
const router = useRouter()
const message = useMessage()

// 表单数据
const form = reactive({
  username: '',
  password: '',
  rememberMe: false
})

// 密码显示状态
const showPassword = ref(false)

// 表单引用
const formRef = ref<InstanceType<typeof NForm>>()

// 登录加载状态
const loading = ref(false)

// 错误信息
const errorMessage = ref('')

// 处理标准登录提交
const handleLogin = async () => {
  if (!formRef.value) return
  
  formRef.value.validate(async (errors) => {
    if (!errors) {
      loading.value = true
      errorMessage.value = ''
      
      try {
        // 调用登录接口
        const response = await AuthService.login({
          username: form.username,
          password: form.password
        })
        
        // 确保token被正确存储，与OAuth登录保持一致
        if (response && response.accessToken) {
          localStorage.setItem('auth_token', response.accessToken)
        }
        
        // 如果选择记住我，存储用户名
        if (form.rememberMe) {
          localStorage.setItem('rememberedUsername', form.username)
        } else {
          localStorage.removeItem('rememberedUsername')
        }
        
        // 登录成功，跳转到首页
        message.success('登录成功')
        router.push('/')
      } catch (error) {
        // 更详细的错误处理
        if (error instanceof Error) {
          errorMessage.value = error.message
          // 处理特定错误类型
          if (error.message.includes('401')) {
            message.error('用户名或密码错误')
          } else {
            message.error(errorMessage.value)
          }
        } else {
          errorMessage.value = '登录失败，请稍后重试'
          message.error(errorMessage.value)
        }
      } finally {
        loading.value = false
      }
    }
  })
}

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