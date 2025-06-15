<template>
  <div class="p-6 rounded-lg w-sm backdrop-blur-xs bg-white/50">
    <!-- 头像占位符 -->
    <div class="flex justify-center mb-1">
      <Icon class="text-gray-600">
        <AccountCircleRound/>
      </Icon>
    </div>

    <!-- 主标题 -->
    <p class="text-gray-700 text-xl font-bold text-center mb-1">您尚未登录</p>

    <!-- 副标题 -->
    <p class="text-gray-500 text-center text-sm">
      登录后，即可云端同步您的个性化设置、网站捷径和便笺
    </p>
  </div>
</template>
<script setup lang="ts">
import {AccountCircleRound} from '@vicons/material'
import {Icon} from '@vicons/utils'
</script>