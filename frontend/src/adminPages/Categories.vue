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
      <!-- 拖拽提示区域 -->
      <div class="text-xs text-gray-500 dark:text-gray-400 mt-2 flex items-center">
        <Icon icon="ep:info-circle" class="h-3 w-3 mr-1" />
        拖动卡片可以调整分类顺序
      </div>
    </div>

    <!-- 分类列表 -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4" 
         @dragover.prevent
         @drop="handleDrop($event, null)">
      <div 
        v-for="(category, index) in filteredCategories" 
        :key="category.key"
        :draggable="true"
        @dragstart="handleDragStart($event, category)"
        @dragover.prevent
        @drop="handleDrop($event, index)"
        @dragenter="(event: DragEvent) => { if (dragOverIndex !== null) dragOverIndex = index; handleDragEnter(event); }"
        @dragleave="handleDragLeave($event)"
        class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6 hover:shadow-md transition-all hover:-translate-y-1 cursor-move"
        :class="{ 'border-primary opacity-70 shadow-md bg-blue-50 dark:bg-blue-900/20': dragOverIndex === index }"
      >
        <div class="flex justify-between items-start mb-4">
          <div 
            class="w-12 h-12 rounded-xl flex items-center justify-center bg-gray-100 dark:bg-gray-700"
          >
            <IconFont v-if="category.icon" :type="category.icon" class="h-6 w-6 text-primary" />
            <Icon v-else icon="ep:folder" class="h-6 w-6 text-primary" />
          </div>
          <div class="flex space-x-2">
            <button 
              @click="editCategory(category)" 
              class="p-2 rounded-lg text-gray-500 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
              title="编辑"
              @mousedown.stop
            >
              <Icon icon="ep:edit" class="h-4 w-4" />
            </button>
            <button 
              @click="deleteCategory(category.key)" 
              class="p-2 rounded-lg text-gray-500 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
              title="删除"
              @mousedown.stop
            >
              <Icon icon="ep:delete" class="h-4 w-4" />
            </button>
          </div>
        </div>
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-1">{{ category.name }}</h3>
        <p class="text-sm text-gray-500 dark:text-gray-400 mb-4">{{ category.description || '暂无描述' }}</p>
        <div class="flex justify-between items-center">
          <span class="text-xs font-medium text-gray-500 dark:text-gray-400">
            {{ category.links?.length || 0 }} 个链接
          </span>
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
          :disabled="saving || !form.name"
          class="px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all disabled:opacity-70 disabled:cursor-not-allowed"
        >
          {{ saving ? '保存中...' : '保存' }}
        </button>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useMessage } from 'naive-ui'
import { NSpin, NModal } from 'naive-ui'
import { Icon } from '@iconify/vue'
import IconFont from '../components/IconFont.vue'
import { CategoryService } from '../services/CategoryService'
import {type CategoryModel} from '../models/category'

// 消息提示
const message = useMessage()

// 状态
const loading = ref(false)
const saving = ref(false)
const showModal = ref(false)
const editingId = ref<string | null>(null)

// 拖拽相关状态
const dragOverIndex = ref<number | null>(null)
const draggedItem = ref<any>(null)
const isDragging = ref(false)
const sortTimeout = ref<number | null>(null)

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

// 数据
const categories = ref<CategoryModel[]>([])

// 表单数据
const form = reactive({
  name: '',
  description: ''
})

// 计算属性
const filteredCategories = computed(() => {
  if (!searchQuery.value) return categories.value
  
  const query = searchQuery.value.toLowerCase()
  return categories.value.filter(category => 
    (category.name && category.name.toLowerCase().includes(query)) ||
    (category.description && category.description.toLowerCase().includes(query))
  )
})

// 拖拽相关方法
const handleDragStart = (event: DragEvent, category: CategoryModel) => {
  isDragging.value = true
  draggedItem.value = category
  if (event.dataTransfer) {
    event.dataTransfer.effectAllowed = 'move'
    event.dataTransfer.setData('text/plain', '') // 必须设置数据才能正常拖拽
  }
}

const handleDragEnter = (event: DragEvent) => {
  event.preventDefault()
}

const handleDragLeave = (event: DragEvent) => {
  event.preventDefault()
  // 修复拖拽离开判断逻辑
  setTimeout(() => {
    dragOverIndex.value = null
  }, 0)
}

const handleDrop = (event: DragEvent, dropIndex: number | null) => {
  event.preventDefault()
  
  if (!isDragging.value || dropIndex === null || !draggedItem.value) {
    dragOverIndex.value = null
    return
  }
  
  const dragIndex = categories.value.findIndex(cat => 
    cat.key === draggedItem.value.key
  )
  
  if (dragIndex !== -1 && dragIndex !== dropIndex) {
    // 更新本地数组顺序
    const newCategories = [...categories.value]
    const [removed] = newCategories.splice(dragIndex, 1)
    newCategories.splice(dropIndex, 0, removed)
    categories.value = newCategories
    
    // 添加视觉反馈
    const dropElement = event.target as HTMLElement
    if (dropElement) {
      dropElement.classList.add('animate-pulse')
      setTimeout(() => {
        dropElement.classList.remove('animate-pulse')
      }, 300)
    }
    
    // 延迟保存以避免频繁请求
    if (sortTimeout.value) {
      clearTimeout(sortTimeout.value)
    }
    
    sortTimeout.value = window.setTimeout(async () => {
      await saveCategoryOrder()
    }, 500)
  }
  
  dragOverIndex.value = null
  isDragging.value = false
  draggedItem.value = null
}

const saveCategoryOrder = async () => {
  try {
    const categoryIds = categories.value.map(cat => cat.key)
    await CategoryService.updateCategorySort(categoryIds)
    message.success('分类顺序已更新')
  } catch (error) {
    message.error('更新分类顺序失败')
    console.error('更新分类顺序失败:', error)
    // 重新加载数据以恢复原始顺序
    await loadCategories()
  }
}

// 方法
const loadCategories = async () => {
  try {
    loading.value = true
    const data = await CategoryService.getAllCategories()
    // 标准化数据格式，确保所有对象都使用一致的属性名
    categories.value = data.map((item: any) => ({
      key: item.key || item.CategoryId || '',
      name: item.name || item.CategoryName || '',
      description: item.description || item.Description || '',
      icon: 'folder-o',
      index: 0,
      links: item.links || []
    }))
  } catch (error) {
    message.error('加载分类失败')
    console.error('加载分类失败:', error)
  } finally {
    loading.value = false
  }
}

const resetForm = () => {
  form.name = ''
  form.description = ''
  editingId.value = null
}

const closeModal = () => {
  showModal.value = false
}

const editCategory = (category: CategoryModel) => {
  editingId.value = category.key
  form.name = category.name || ''
  form.description = category.description || ''
  showModal.value = true
}

const saveCategory = async () => {
  if (!form.name) {
    message.warning('请填写分类名称')
    return
  }
  
  try {
    saving.value = true
    
    // 准备分类数据，确保类型安全
    const categoryData: CategoryModel = {
      key: editingId.value || '',
      name: form.name,
      description: form.description,
      icon: 'folder-o',
      index: 0,
      links: []
    }
    
    if (editingId.value) {
      // 编辑模式 - 使用正确的参数格式
      await CategoryService.updateCategory(categoryData)
      message.success('更新成功')
    } else {
      // 创建模式
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
  const category = categories.value.find(cat => cat.key === id)
  if (category && (category.links?.length || 0) > 0) {
    message.warning(`此分类下有 ${category.links.length} 个链接，请先移除这些链接后再删除分类`)
    return
  }
  
  // 使用原生confirm对话框确认删除
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
  height: 8px;
}

::-webkit-scrollbar-track {
  background: transparent;
}

::-webkit-scrollbar-thumb {
  background: #cbd5e0;
  border-radius: 4px;
  transition: background 0.2s;
}

::-webkit-scrollbar-thumb:hover {
  background: #a0aec0;
}

.dark ::-webkit-scrollbar-thumb {
  background: #4a5568;
}

.dark ::-webkit-scrollbar-thumb:hover {
  background: #718096;
}

/* 拖拽状态样式 */
.cursor-move {
  transition: all 0.2s ease;
}

.cursor-move:active {
  cursor: grabbing;
}

/* 暗黑模式优化 */
.dark .bg-blue-900\/20 {
  background-color: rgba(30, 64, 175, 0.2);
}

/* 响应式调整 */
@media (max-width: 768px) {
  .grid {
    grid-template-columns: 1fr;
  }
  
  .p-6 {
    padding: 1.25rem;
  }
  
  .w-12.h-12 {
    width: 3rem;
    height: 3rem;
  }
  
  .h-6.w-6 {
    width: 1.25rem;
    height: 1.25rem;
  }
}

@media (min-width: 769px) and (max-width: 1024px) {
  .grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

/* 按钮交互效果增强 */
button {
  transition: all 0.15s ease;
}

button:active {
  transform: scale(0.95);
}
</style>