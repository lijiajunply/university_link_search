<script setup lang="ts">
import { useRouter } from 'vue-router'
import { NButton, NCard, NResult, useMessage } from 'naive-ui'

const router = useRouter()
const message = useMessage()

const handleClick = () => {
  message.success('按钮被点击！')
  // 可以添加导航逻辑
  router.push('/')
}
</script>

<template>
  <div class="min-h-screen w-full flex items-center justify-center bg-linear-to-br from-blue-50 to-indigo-100 dark:from-gray-800 dark:to-gray-900 p-4">
    <NCard 
      class="w-full max-w-md backdrop-blur-sm bg-white/80 dark:bg-gray-800/80 border border-gray-200 dark:border-gray-700 rounded-2xl shadow-xl"
      :bordered="false"
    >
      <NResult 
        status="404" 
        title="页面未找到" 
        description="抱歉，您访问的页面不存在"
        class="py-4"
      >
        <template #footer>
          <div class="flex flex-col gap-3">
            <NButton 
              type="primary" 
              size="large" 
              @click="handleClick"
              class="w-full"
              secondary
            >
              返回首页
            </NButton>
            <NButton 
              type="tertiary" 
              size="large" 
              @click="router.go(-1)"
              class="w-full"
            >
              返回上一页
            </NButton>
          </div>
        </template>
      </NResult>
    </NCard>
  </div>
</template>

<style scoped>
/* 移除原有的样式，使用 TailwindCSS 类 */
</style>