<template>
  <div class="py-6 px-4 sm:px-6 lg:px-8">
    <!-- 页面标题 -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6">
      <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">分类管理</h1>
      <button 
        @click="showModal = true" 
        class="mt-4 sm:mt-0 inline-flex items-center px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all"
      >
        <Icon icon="ep:plus" class="mr-2 h-4 w-4" />
        添加分类
      </button>
    </div>

    <!-- 搜索框 -->
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-4 mb-6">
      <div class="relative">
        <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
          <Icon icon="ep:search" class="h-5 w-5 text-gray-400" />
        </div>
        <input
          v-model="searchQuery"
          type="text"
          class="pl-10 block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          placeholder="搜索分类名称"
        />
        <button 
          @click="loadCategories" 
          class="absolute inset-y-0 right-0 pr-3 flex items-center"
        >
          <Icon icon="ep:refresh" class="h-5 w-5 text-gray-400 hover:text-gray-600 dark:hover:text-gray-200 transition-colors"
            :class="{ 'animate-spin': loading }" />
        </button>
      </div>
    </div>

    <!-- 分类列表 -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div 
        v-for="category in filteredCategories" 
        :key="category.CategoryId" 
        class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6 hover:shadow-md transition-all hover:-translate-y-1"
      >
        <div class="flex justify-between items-start mb-4">
          <div 
            class="w-12 h-12 rounded-xl flex items-center justify-center" 
            :style="{ backgroundColor: category.Color + '20' }"
          >
            <Icon icon="ep:folder" class="h-6 w-6" :style="{ color: category.Color }" />
          </div>
          <div class="flex space-x-2">
            <button 
              @click="editCategory(category)" 
              class="p-2 rounded-lg text-gray-500 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
              title="编辑"
            >
              <Icon icon="ep:edit" class="h-4 w-4" />
            </button>
            <button 
              @click="deleteCategory(category.CategoryId)" 
              class="p-2 rounded-lg text-gray-500 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
              title="删除"
            >
              <Icon icon="ep:delete" class="h-4 w-4" />
            </button>
          </div>
        </div>
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-1">{{ category.CategoryName }}</h3>
        <p class="text-sm text-gray-500 dark:text-gray-400 mb-4">{{ category.Description || '暂无描述' }}</p>
        <div class="flex justify-between items-center">
          <span class="text-xs font-medium text-gray-500 dark:text-gray-400">
            {{ category.linkCount || 0 }} 个链接
          </span>
          <div class="flex items-center space-x-1">
            <span 
              class="w-3 h-3 rounded-full" 
              :style="{ backgroundColor: category.Color }"
              title="分类颜色"
            ></span>
            <span class="text-xs font-mono text-gray-500 dark:text-gray-400">{{ category.Color }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- 空状态 -->
    <div v-if="categories.length === 0 && !loading" class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-8 text-center mt-6">
      <Icon icon="ep:folder-open" class="h-12 w-12 text-gray-400 mx-auto mb-4" />
      <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-1">暂无分类</h3>
      <p class="text-gray-500 dark:text-gray-400 mb-4">点击"添加分类"按钮创建您的第一个分类</p>
      <button 
        @click="showModal = true" 
        class="inline-flex items-center px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all"
      >
        <Icon icon="ep:plus" class="mr-2 h-4 w-4" />
        添加分类
      </button>
    </div>

    <!-- 加载状态 -->
    <div v-if="loading" class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-8 text-center mt-6">
      <n-spin size="large" />
      <p class="mt-4 text-gray-500 dark:text-gray-400">加载中...</p>
    </div>

    <!-- 创建/编辑分类对话框 -->
    <n-modal
      v-model:show="isModalVisible"
      :title="editingId ? '编辑分类' : '创建分类'"
      preset="dialog"
      size="medium"
    >
      <div class="space-y-4">
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">分类名称 <span class="text-danger">*</span></label>
          <input
            v-model="form.name"
            type="text"
            placeholder="请输入分类名称"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">分类颜色 <span class="text-danger">*</span></label>
          <div class="flex items-center space-x-3">
            <input
              v-model="form.color"
              type="color"
              class="w-10 h-10 rounded-lg border-0 cursor-pointer"
              title="选择颜色"
            />
            <input
              v-model="form.color"
              type="text"
              placeholder="#000000"
              class="flex-grow rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
            />
          </div>
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">分类描述</label>
          <textarea
            v-model="form.description"
            placeholder="请输入分类描述"
            rows="3"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all resize-none"
          ></textarea>
        </div>
        <div class="flex items-center space-x-2 pt-2">
          <Icon icon="ep:info-circle" class="h-5 w-5 text-info" />
          <span class="text-sm text-gray-500 dark:text-gray-400">创建分类后，可以在链接管理中使用这些分类进行归类</span>
        </div>
      </div>
      <template #footer>
        <button 
          @click="closeModal" 
          class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg shadow-sm text-sm font-medium text-gray-700 dark:text-gray-200 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all mr-2"
        >
          取消
        </button>
        <button 
          @click="saveCategory" 
          :disabled="saving || !form.name || !form.color"
          class="px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all disabled:opacity-70 disabled:cursor-not-allowed"
        >
          {{ saving ? '保存中...' : '保存' }}
        </button>
      </template>
    </n-modal>

    <!-- 颜色选择器 -->
    <div class="fixed bottom-4 right-4 bg-white dark:bg-gray-800 rounded-xl shadow-lg border border-gray-200 dark:border-gray-700 p-3 z-10">
      <h4 class="text-xs font-medium text-gray-500 dark:text-gray-400 mb-2">快速颜色</h4>
      <div class="grid grid-cols-6 gap-2">
        <button
          v-for="color in presetColors"
          :key="color"
          @click="selectPresetColor(color)"
          class="w-6 h-6 rounded-lg transition-transform hover:scale-110 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50"
          :style="{ backgroundColor: color }"
          :title="color"
        ></button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useMessage } from 'naive-ui'
import { NSpin, NModal } from 'naive-ui'
import { Icon } from '@iconify/vue'
import { CategoryService } from '../services/CategoryService'
import type { CategoryModel } from '../models/category'

// 消息提示
const message = useMessage()

// 状态
const loading = ref(false)
const saving = ref(false)
const showModal = ref(false)
const editingId = ref<string | null>(null)

// 计算属性用于处理v-model
const isModalVisible = computed({
  get: () => showModal.value,
  set: (value) => {
    if (!value) {
      showModal.value = false
      resetForm()
      editingId.value = null
    }
  }
})

// 搜索
const searchQuery = ref('')

// 预设颜色
const presetColors = [
  '#0071e3', // 蓝色
  '#34c759', // 绿色
  '#ff9500', // 橙色
  '#ff3b30', // 红色
  '#5856d6', // 紫色
  '#af52de', // 粉色
  '#ff2d55', // 玫瑰红
  '#5ac8fa', // 浅蓝
  '#ffcc00', // 黄色
  '#00c7be', // 青色
  '#a2845e', // 棕色
  '#8e8e93'  // 灰色
]

// 数据
const categories = ref<any[]>([])

// 表单数据
const form = reactive({
  name: '',
  color: '#0071e3',
  description: ''
})

// 计算属性
const filteredCategories = computed(() => {
  if (!searchQuery.value) return categories.value
  
  const query = searchQuery.value.toLowerCase()
  return categories.value.filter(category => 
    category.CategoryName.toLowerCase().includes(query) ||
    (category.Description && category.Description.toLowerCase().includes(query))
  )
})

// 方法
const loadCategories = async () => {
  try {
    loading.value = true
    categories.value = await CategoryService.getAllCategories()
  } catch (error) {
    message.error('加载分类失败')
    console.error('加载分类失败:', error)
  } finally {
    loading.value = false
  }
}

const resetForm = () => {
  form.name = ''
  form.color = '#0071e3'
  form.description = ''
  editingId.value = null
}

const closeModal = () => {
  showModal.value = false
}

const editCategory = (category: any) => {
  editingId.value = category.CategoryId
  form.name = category.CategoryName
  form.color = category.Color || '#0071e3'
  form.description = category.Description || ''
  showModal.value = true
}

const saveCategory = async () => {
  if (!form.name || !form.color) {
    message.warning('请填写分类名称和颜色')
    return
  }
  
  try {
    saving.value = true
    
    if (editingId.value) {
      // 编辑模式
      // 转换为CategoryModel格式
      const categoryData: CategoryModel = {
        key: editingId.value,
        name: form.name,
        description: form.description,
        icon: '',
        index: 0,
        links: []
      };
      await CategoryService.updateCategory(categoryData)
      message.success('更新成功')
    } else {
      // 创建模式
      // 转换为CategoryModel格式
      const categoryData: CategoryModel = {
        key: '',
        name: form.name,
        description: form.description,
        icon: '',
        index: 0,
        links: []
      };
      await CategoryService.createCategory(categoryData)
      message.success('创建成功')
    }
    
    await loadCategories()
    closeModal()
  } catch (error) {
    message.error(editingId.value ? '更新失败' : '创建失败')
    console.error(editingId.value ? '更新分类失败:' : '创建分类失败:', error)
  } finally {
    saving.value = false
  }
}

const deleteCategory = async (id: string) => {
  // 检查是否有链接使用此分类
  const category = categories.value.find(cat => cat.CategoryId === id)
  if (category && category.linkCount > 0) {
    message.warning(`此分类下有 ${category.linkCount} 个链接，请先移除这些链接后再删除分类`)
    return
  }
  
  if (confirm('确定要删除这个分类吗？')) {
    try {
      await CategoryService.deleteCategory(id)
      message.success('删除成功')
      await loadCategories()
    } catch (error) {
      message.error((error as Error).message || '删除失败')
      console.error('删除分类失败:', error)
    }
  }
}

const selectPresetColor = (color: string) => {
  form.color = color
}

// 生命周期
onMounted(async () => {
  await loadCategories()
})
</script>

<style scoped>
/* 自定义动画 */
@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

/* 自定义滚动条 */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: transparent;
}

::-webkit-scrollbar-thumb {
  background: #cbd5e0;
  border-radius: 4px;
}

.dark ::-webkit-scrollbar-thumb {
  background: #4a5568;
}

/* 响应式调整 */
@media (max-width: 768px) {
  .grid {
    grid-template-columns: 1fr;
  }
}
</style>