<template>
  <div class="mx-auto px-16 py-4">
    <footer class="layout-footer text-center p-8">
      <p class="layout-footer-title mb-4">
        &copy; 2024 iOS Club of XAUAT
      </p>
      <div class="flex justify-center space-x-4">
        <div class="flex items-center space-x-2">
          <a class="a" href="https://cn.xauat.edu.cn/" target="_blank">
            西安建筑科技大学
          </a>
          <svg class="w-4 h-4 text-gray-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
               stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
          </svg>
        </div>
        <div class="flex items-center space-x-2">
          <a class="a" href="https://beian.miit.gov.cn/" target="_blank">
            备案号 陕ICP备2024031872号
          </a>
          <svg class="w-4 h-4 text-gray-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
               stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
          </svg>
        </div>
        <div class="flex items-center space-x-2">
          <a class="a" href="https://gitee.com/XAUATiOSClub" target="_blank">
            我们的Gitee组织
          </a>
          <svg class="w-4 h-4 text-gray-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
               stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
          </svg>
        </div>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.layout-footer {
  background: rgba(255, 255, 255, 0.3);
  -webkit-backdrop-filter: blur(20px);
  backdrop-filter: blur(20px);
  box-shadow: inset 0 0 6px rgba(255, 255, 255, 0.3);
  padding-left: 10px;
  padding-right: 10px;

  border-radius: 10px;
}

.layout-footer-title {
  color: #f3f5f7;
}

.a {
  color: #f3f5f7;
  transition: .2s;
  text-decoration: none;
  background: linear-gradient(#07f2, #07f2) no-repeat center bottom / 100% 0;
}

.a:hover {
  color: #f3f5f7;
  background: linear-gradient(#07f2, #07f2) no-repeat center bottom / 100% 100%;
  border-radius: .2em;
}
</style>
<script setup lang="ts">
</script>