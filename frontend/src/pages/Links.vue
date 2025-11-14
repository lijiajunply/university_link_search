<template>
  <div class="py-6 px-4 sm:px-6 lg:px-8">
    <!-- 页面标题 -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6">
      <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">链接管理</h1>
      <button 
        @click="showCreateModal = true" 
        class="mt-4 sm:mt-0 inline-flex items-center px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all"
      >
        <Icon icon="ep:plus" class="mr-2 h-4 w-4" />
        添加链接
      </button>
    </div>

    <!-- 搜索和筛选 -->
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-4 mb-6">
      <div class="flex flex-col lg:flex-row gap-4">
        <div class="relative flex-grow">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Icon icon="ep:search" class="h-5 w-5 text-gray-400" />
          </div>
          <input
            v-model="searchQuery"
            type="text"
            class="pl-10 block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
            placeholder="搜索链接名称、URL或标签"
          />
        </div>
        <div class="flex gap-2">
          <select
            v-model="filterCategory"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          >
            <option value="">所有分类</option>
            <option v-for="category in categories" :key="category.CategoryId" :value="category.CategoryId">
              {{ category.CategoryName }}
            </option>
          </select>
          <button 
            @click="loadLinks" 
            class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg shadow-sm text-sm font-medium text-gray-700 dark:text-gray-200 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all"
          >
            <Icon icon="ep:refresh" class="h-4 w-4 mr-2" />
            刷新
          </button>
        </div>
      </div>
    </div>

    <!-- 链接列表 -->
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                ID
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                名称
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                URL
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                分类
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                标签
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                访问次数
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                创建时间
              </th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                操作
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="link in filteredLinks" :key="link.id" class="hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ link.id }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">
                {{ link.name }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-500 dark:text-gray-400 max-w-xs truncate">
                <a :href="link.url" target="_blank" rel="noopener noreferrer" class="text-primary hover:underline">
                  {{ link.url }}
                </a>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200">
                  {{ getCategoryName(link.categoryId) }}
                </span>
              </td>
              <td class="px-6 py-4 text-sm text-gray-500 dark:text-gray-400">
                <span v-for="(tag, index) in link.tags" :key="index" class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200 mr-1 mb-1">
                  {{ tag }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                {{ link.visitCount }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                {{ formatDate(link.createdAt) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button 
              @click="editLink(link)" 
              class="text-primary hover:text-primary/80 mr-3 transition-colors"
            >
              <Icon icon="ep:edit" class="h-4 w-4" />
            </button>
            <button 
              @click="deleteLink(link.Key || link.id)" 
              class="text-danger hover:text-danger/80 transition-colors"
            >
              <Icon icon="ep:delete" class="h-4 w-4" />
            </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- 空状态 -->
      <div v-if="links.length === 0 && !loading" class="p-8 text-center">
        <Icon icon="ep:document" class="h-12 w-12 text-gray-400 mx-auto mb-4" />
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-1">暂无链接</h3>
        <p class="text-gray-500 dark:text-gray-400">点击"添加链接"按钮创建您的第一个链接</p>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="p-8 text-center">
        <n-spin size="large" />
        <p class="mt-4 text-gray-500 dark:text-gray-400">加载中...</p>
      </div>
    </div>

    <!-- 分页 -->
    <div v-if="links.length > 0 && !loading" class="mt-4 flex justify-between items-center">
      <p class="text-sm text-gray-500 dark:text-gray-400">
        显示 {{ startIndex + 1 }} 到 {{ Math.min(endIndex, links.length) }} 条，共 {{ links.length }} 条
      </p>
      <div class="flex space-x-2">
        <button 
          @click="prevPage" 
          :disabled="currentPage === 1"
          class="px-3 py-1 rounded-lg text-sm font-medium transition-all"
          :class="currentPage === 1 ? 'text-gray-400 cursor-not-allowed' : 'text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700'"
        >
          上一页
        </button>
        <button 
          @click="nextPage" 
          :disabled="endIndex >= links.length"
          class="px-3 py-1 rounded-lg text-sm font-medium transition-all"
          :class="endIndex >= links.length ? 'text-gray-400 cursor-not-allowed' : 'text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700'"
        >
          下一页
        </button>
      </div>
    </div>

    <!-- 创建/编辑链接对话框 -->
    <n-modal
      v-model:show="isModalVisible"
      :title="showEditModal ? '编辑链接' : '创建链接'"
      preset="dialog"
      size="large"
    >
      <div class="space-y-4">
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">链接名称</label>
          <input
            v-model="form.name"
            type="text"
            placeholder="请输入链接名称"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">URL地址</label>
          <input
            v-model="form.url"
            type="url"
            placeholder="https://"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">分类</label>
          <select
            v-model="form.categoryId"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          >
            <option value="">请选择分类</option>
            <option v-for="category in categories" :key="category.CategoryId" :value="category.CategoryId">
              {{ category.CategoryName }}
            </option>
          </select>
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">标签 (用逗号分隔)</label>
          <input
            v-model="formTags"
            type="text"
            placeholder="标签1,标签2,标签3"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          />
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
          @click="saveLink" 
          :disabled="saving || !form.name || !form.url"
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
import { LinkService } from '../services/LinkService'
import { CategoryService } from '../services/CategoryService'

// 自定义表单类型
interface LinkForm {
  name: string
  url: string
  categoryId: string
  tags: string[]
}

// 消息提示
const message = useMessage()

// 状态
const loading = ref(false)
const saving = ref(false)
const showCreateModal = ref(false)
const showEditModal = ref(false)
const editingId = ref<number | null>(null)

// 计算属性 - 用于modal的显示控制
const isModalVisible = computed({
  get: () => showCreateModal.value || showEditModal.value,
  set: (value) => {
    if (!value) {
      showCreateModal.value = false
      showEditModal.value = false
    }
  }
})

// 分页
const currentPage = ref(1)
const pageSize = 10

// 搜索和筛选
const searchQuery = ref('')
const filterCategory = ref('')

// 数据
const links = ref<any[]>([])
const categories = ref<any[]>([])

// 表单数据
const form = reactive<LinkForm>({
  name: '',
  url: '',
  categoryId: '',
  tags: []
})
const formTags = ref('')

// 计算属性
const filteredLinks = computed(() => {
  let result = [...links.value]
  
  // 按搜索词过滤
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(link => 
      link.name.toLowerCase().includes(query) ||
      link.url.toLowerCase().includes(query) ||
      link.tags.some((tag: string) => tag.toLowerCase().includes(query))
    )
  }
  
  // 按分类过滤
  if (filterCategory.value) {
    result = result.filter(link => link.categoryId === filterCategory.value)
  }
  
  return result
})

const startIndex = computed(() => (currentPage.value - 1) * pageSize)
const endIndex = computed(() => startIndex.value + pageSize)

// 加载链接数据
const loadLinks = async () => {
  try {
    loading.value = true
    links.value = await LinkService.getAllLinks()
  } catch (error) {
    message.error('加载链接失败')
    console.error('加载链接失败:', error)
  } finally {
    loading.value = false
  }
}

// 加载分类数据
const loadCategories = async () => {
  try {
    categories.value = await CategoryService.getAllCategories()
  } catch (error) {
    message.error('加载分类失败')
    console.error('加载分类失败:', error)
  }
}

const resetForm = () => {
  form.name = ''
  form.url = ''
  form.categoryId = ''
  form.tags = []
  formTags.value = ''
  editingId.value = null
}

const editLink = (link: any) => {
  editingId.value = link.id
  form.name = link.name
  form.url = link.url
  form.categoryId = link.categoryId
  form.tags = [...link.tags]
  formTags.value = form.tags.join(', ')
  showEditModal.value = true
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  resetForm()
}

const saveLink = async () => {
  if (!form.name || !form.url) {
    message.warning('请填写完整信息')
    return
  }
  
  // 处理标签
  form.tags = formTags.value
    .split(',')
    .map(tag => tag.trim())
    .filter(tag => tag)
  
  try {
    saving.value = true
    
    if (editingId.value) {
      // 编辑模式
      const updateData: any = {
        id: editingId.value,
        name: form.name,
        url: form.url,
        categoryId: form.categoryId,
        tags: form.tags.join(',')
      };
      await LinkService.updateLink(updateData)
      message.success('更新成功')
    } else {
      // 创建模式
      const linkData: any = {
        name: form.name,
        url: form.url,
        categoryId: form.categoryId,
        tags: form.tags.join(',')
      };
      await LinkService.createLink(linkData)
      message.success('创建成功')
    }
    
    await loadLinks()
    closeModal()
  } catch (error) {
    message.error(editingId.value ? '更新失败' : '创建失败')
    console.error('保存链接失败:', error)
  } finally {
    saving.value = false
  }
}

const deleteLink = async (id: string) => {
  if (confirm('确定要删除这个链接吗？')) {
    try {
      await LinkService.deleteLink(id)
      message.success('删除成功')
      await loadLinks()
    } catch (error) {
      message.error((error as Error).message || '删除失败')
    }
  }
}

const prevPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

const nextPage = () => {
  if (endIndex.value < filteredLinks.value.length) {
    currentPage.value++
  }
}

const getCategoryName = (categoryId: string): string => {
  const category = categories.value.find(cat => String(cat.CategoryId) === String(categoryId))
  return category ? category.CategoryName : '未分类'
}

const formatDate = (dateString: string): string => {
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 生命周期
onMounted(async () => {
  await Promise.all([loadLinks(), loadCategories()])
})
</script>

<style scoped>
/* 自定义样式 */
input:focus,
select:focus {
  transform: translateY(-1px);
}

button {
  transition-property: background-color, border-color, color, fill, stroke, opacity, box-shadow, transform;
  transition-duration: 150ms;
}
</style>