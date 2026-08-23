<template>
  <div class="space-y-6 animate-fade-in">
    <!-- 顶部标题栏 -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div class="flex items-center gap-4">
        <button
          @click="router.back()"
          class="w-10 h-10 rounded-xl bg-white dark:bg-[#1f1f23] border border-[var(--border-primary)] flex items-center justify-center hover:bg-[var(--hover-bg)] transition-colors shadow-sm">
          <Icon icon="solar:arrow-left-bold" class="w-5 h-5 text-[var(--text-secondary)]" />
        </button>
        <div>
          <h1 class="text-2xl sm:text-3xl font-bold text-[var(--text-primary)] tracking-tight flex items-center gap-2">
            <span>{{ categoryName || '加载中...' }}</span>
            <span class="text-base font-normal text-[var(--text-tertiary)] bg-[var(--bg-secondary)] px-2 py-0.5 rounded-lg">链接管理</span>
          </h1>
          <p class="mt-1 text-sm text-[var(--text-secondary)]">管理该分类下的所有导航链接</p>
        </div>
      </div>
      <button
        @click="handleCreate"
        class="flex items-center justify-center gap-2 px-4 py-2.5 bg-blue-500 hover:bg-blue-600 active:bg-blue-700 text-white rounded-xl font-medium transition-all duration-200 shadow-lg shadow-blue-500/20 hover:shadow-blue-500/40 active:scale-95">
        <Icon icon="solar:add-circle-bold" class="w-5 h-5" />
        <span>添加链接</span>
      </button>
    </div>

    <!-- 数据表格区域 -->
    <div 
      ref="tableContainer"
      class="bg-white/80 dark:bg-[#18181c]/80 backdrop-blur-xl rounded-2xl border border-[var(--border-primary)] shadow-sm overflow-hidden">
      <n-data-table
        :columns="columns"
        :data="links"
        :loading="loading"
        :bordered="false"
        :single-line="false"
        :row-key="(row) => row.key"
        class="custom-table"
      />
    </div>

    <!-- 编辑/创建弹窗 -->
    <n-modal
      v-model:show="showModal"
      :mask-closable="false"
      transform-origin="center">
      <div class="w-full max-w-lg bg-white dark:bg-[#1f1f23] rounded-2xl shadow-2xl border border-[var(--border-primary)] overflow-hidden">
        <!-- 弹窗标题 -->
        <div class="px-6 py-4 border-b border-[var(--border-primary)] flex items-center justify-between bg-[var(--bg-secondary)]/50">
          <h3 class="text-lg font-bold text-[var(--text-primary)]">
            {{ isEdit ? '编辑链接' : '添加链接' }}
          </h3>
          <button 
            @click="showModal = false"
            class="p-1 rounded-lg hover:bg-[var(--hover-bg)] text-[var(--text-tertiary)] hover:text-[var(--text-primary)] transition-colors">
            <Icon icon="solar:close-circle-bold" class="w-6 h-6" />
          </button>
        </div>

        <!-- 表单内容 -->
        <div class="p-6">
          <n-form
            ref="formRef"
            :model="formModel"
            :rules="rules"
            label-placement="top"
            require-mark-placement="right-hanging">
            
            <n-form-item label="链接名称" path="name">
              <n-input v-model:value="formModel.name" placeholder="例如：GitHub" class="custom-input">
                <template #prefix>
                  <Icon icon="solar:text-bold" class="text-[var(--text-tertiary)]" />
                </template>
              </n-input>
            </n-form-item>

            <n-form-item label="链接地址 (URL)" path="url">
              <n-input v-model:value="formModel.url" placeholder="https://..." class="custom-input">
                <template #prefix>
                  <Icon icon="solar:link-bold" class="text-[var(--text-tertiary)]" />
                </template>
              </n-input>
            </n-form-item>

            <n-form-item label="图标 (iconfont / URL)" path="icon">
              <div class="flex gap-3 w-full">
                <div class="flex-1">
                  <n-input v-model:value="formModel.icon" placeholder="iconfont 类名或图片链接，例如：github" class="custom-input">
                    <template #prefix>
                      <Icon icon="solar:sticker-smile-circle-2-bold" class="text-[var(--text-tertiary)]" />
                    </template>
                  </n-input>
                </div>
                <div class="flex-shrink-0 w-10 h-10 rounded-xl bg-[var(--bg-secondary)] border border-[var(--border-primary)] flex items-center justify-center overflow-hidden text-(--text-primary)">
                  <AppLogo v-if="formModel.icon" :icon="formModel.icon || ''" :name="formModel.name" :url="formModel.url" :size="24" />
                </div>
              </div>
            </n-form-item>

            <n-form-item label="描述" path="description">
              <n-input
                v-model:value="formModel.description"
                type="textarea"
                placeholder="简要描述该链接..."
                :autosize="{ minRows: 2, maxRows: 4 }"
                class="custom-input"
              />
            </n-form-item>
          </n-form>
        </div>

        <!-- 底部按钮 -->
        <div class="px-6 py-4 bg-[var(--bg-secondary)]/30 flex justify-end gap-3">
          <button
            @click="showModal = false"
            class="px-4 py-2 rounded-xl text-sm font-medium text-[var(--text-secondary)] hover:bg-[var(--hover-bg)] transition-colors">
            取消
          </button>
          <button
            @click="handleSubmit"
            :disabled="submitting"
            class="px-4 py-2 rounded-xl text-sm font-medium bg-blue-500 hover:bg-blue-600 text-white shadow-lg shadow-blue-500/20 transition-all active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed">
            <div class="flex items-center gap-2">
              <Icon v-if="submitting" icon="svg-spinners:ring-resize" />
              <span>{{ isEdit ? '保存更改' : '立即创建' }}</span>
            </div>
          </button>
        </div>
      </div>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { h, onMounted, ref, reactive, nextTick, watch } from 'vue'
import { 
  NDataTable, NModal, NForm, NFormItem, NInput,
  useMessage, useDialog, type DataTableColumns, type FormInst
} from 'naive-ui'
import { Icon } from '@iconify/vue'
import Sortable from 'sortablejs'
import { useRoute, useRouter } from 'vue-router'
import { LinkService } from '../services/LinkService'
import { CategoryService } from '../services/CategoryService'
import type { LinkModel } from '../models/link'
import AppLogo from '../components/AppLogo.vue'

const route = useRoute()
const router = useRouter()
const message = useMessage()
const dialog = useDialog()

// 状态定义
const loading = ref(false)
const submitting = ref(false)
const showModal = ref(false)
const isEdit = ref(false)
const categoryName = ref('')
const links = ref<LinkModel[]>([])
const formRef = ref<FormInst | null>(null)
const tableContainer = ref<HTMLElement | null>(null)

// 当前分类ID
const categoryId = route.params.id as string

// 表单数据
const defaultForm: LinkModel = {
  key: '',
  name: '',
  url: '',
  icon: '',
  index: 0,
  description: ''
}
const formModel = reactive<LinkModel>({ ...defaultForm })

// 表单校验规则
const rules = {
  name: { required: true, message: '请输入链接名称', trigger: 'blur' },
  url: { required: true, message: '请输入链接地址', trigger: 'blur' }
}

// 表格列定义
const columns: DataTableColumns<LinkModel> = [
  {
    key: 'drag',
    width: 40,
    render() {
      return h(Icon, { 
        icon: 'solar:hamburger-menu-linear', 
        class: 'drag-handle cursor-move w-5 h-5 text-[var(--text-tertiary)] hover:text-[var(--text-primary)] transition-colors' 
      })
    }
  },
  {
    title: '图标',
    key: 'icon',
    width: 60,
    render(row) {
      if (!row.icon) return null
      return h('div', { class: 'w-10 h-10 rounded-xl bg-gray-50 dark:bg-gray-800 border border-[var(--border-primary)] flex items-center justify-center overflow-hidden text-(--text-primary)' }, [
        h(AppLogo, { icon: row.icon, name: row.name, url: row.url, size: 24 })
      ])
    }
  },
  {
    title: '链接名称',
    key: 'name',
    render(row) {
      return h('div', { class: 'flex flex-col' }, [
        h('a', { 
          href: row.url, 
          target: '_blank',
          class: 'font-semibold text-[var(--text-primary)] hover:text-blue-500 transition-colors truncate max-w-[200px]' 
        }, row.name),
        h('span', { class: 'text-xs text-[var(--text-tertiary)] truncate max-w-[200px]' }, row.url)
      ])
    }
  },
  {
    title: 'Key',
    key: 'key',
    width: 100,
    render(row) {
       return h('span', { class: 'text-xs font-mono bg-[var(--bg-secondary)] px-2 py-1 rounded text-[var(--text-secondary)]' }, row.key)
    }
  },
  {
    title: '描述',
    key: 'description',
    render(row) {
      return h('span', { class: 'text-[var(--text-secondary)] text-sm line-clamp-1' }, row.description || '-')
    }
  },
  {
    title: '排序',
    key: 'index',
    width: 80,
    render(row) {
      return h('div', { class: 'font-mono text-[var(--text-secondary)]' }, row.index)
    }
  },
  {
    title: '操作',
    key: 'actions',
    width: 150,
    render(row) {
      return h('div', { class: 'flex gap-2' }, [
        // 编辑按钮
        h('button', {
          class: 'p-2 rounded-lg hover:bg-orange-500/10 text-orange-500 transition-colors',
          title: '编辑',
          onClick: () => handleEdit(row)
        }, [h(Icon, { icon: 'solar:pen-bold', class: 'w-5 h-5' })]),
        
        // 删除按钮
        h('button', {
          class: 'p-2 rounded-lg hover:bg-red-500/10 text-red-500 transition-colors',
          title: '删除',
          onClick: () => handleDelete(row)
        }, [h(Icon, { icon: 'solar:trash-bin-trash-bold', class: 'w-5 h-5' })])
      ])
    }
  }
]

// 获取数据
const fetchData = async () => {
  if (!categoryId) return
  loading.value = true
  try {
    // 并行获取分类详情和链接列表
    const [category, linksData] = await Promise.all([
      CategoryService.getCategoryById(categoryId).catch(() => ({ name: '未知分类' })),
      LinkService.getLinksByCategory(categoryId)
    ])
    categoryName.value = (category as any).name
    links.value = linksData
  } catch (error: any) {
    message.error(error.message || '获取数据失败')
  } finally {
    loading.value = false
  }
}

// 打开创建弹窗
const handleCreate = () => {
  isEdit.value = false
  Object.assign(formModel, defaultForm)
  showModal.value = true
}

// 打开编辑弹窗
const handleEdit = (row: LinkModel) => {
  isEdit.value = true
  Object.assign(formModel, row)
  showModal.value = true
}

// 处理删除
const handleDelete = (row: LinkModel) => {
  dialog.warning({
    title: '确认删除',
    content: `确定要删除链接 "${row.name}" 吗？`,
    positiveText: '确认删除',
    negativeText: '取消',
    onPositiveClick: async () => {
      try {
        // 假设 Model 的 key 并不是 ID，但是 Service deleteLink 需要 ID。
        // 通常后端 ID 字段是隐藏的。尝试使用 (row as any)._id 或 (row as any).id
        const idToDelete = (row as any)._id || (row as any).id || row.key
        await LinkService.deleteLink(idToDelete)
        message.success('删除成功')
        fetchData()
      } catch (error: any) {
        message.error(error.message || '删除失败')
      }
    }
  })
}

// 提交表单
const handleSubmit = (e: MouseEvent) => {
  e.preventDefault()
  formRef.value?.validate(async (errors) => {
    if (!errors) {
      submitting.value = true
      try {
        // 构造 payload，注入 categoryKey（后端 LinkModel.CategoryKey 用于关联分类）
        const payload = {
          ...formModel,
          categoryKey: categoryId
        }

        if (isEdit.value) {
          await LinkService.updateLink(payload)
          message.success('更新成功')
        } else {
          await LinkService.createLink(payload)
          message.success('创建成功')
        }
        showModal.value = false
        fetchData()
      } catch (error: any) {
        message.error(error.message || '操作失败')
      } finally {
        submitting.value = false
      }
    }
  })
}

// 初始化拖拽排序
const sortableInstance = ref<Sortable | null>(null)

const initSortable = () => {
  if (!tableContainer.value) return
  
  const el = tableContainer.value.querySelector('.n-data-table-tbody') as HTMLElement
  if (!el) return

  // 如果已经初始化过，先销毁
  if (sortableInstance.value) {
    sortableInstance.value.destroy()
    sortableInstance.value = null
  }

  sortableInstance.value = Sortable.create(el, {
    handle: '.drag-handle',
    animation: 150,
    ghostClass: 'bg-blue-500/10',
    onEnd: async (evt) => {
      const { oldIndex, newIndex } = evt
      if (oldIndex === undefined || newIndex === undefined || oldIndex === newIndex) return

      // 移动数组元素
      const item = links.value.splice(oldIndex, 1)[0]
      if (item !== undefined) {
        links.value.splice(newIndex, 0, item)

        // 更新本地索引显示（可选）
        links.value.forEach((link, idx) => {
          link.index = idx
        })

        // 调用API更新排序
        try {
          // 假设后端接受 key 数组作为排序依据，并且需要 categoryId
          const sortedIds = links.value.map(l => l.key)
          await LinkService.updateLinkSort(categoryId, sortedIds)
          message.success('排序已更新')
        } catch (error: any) {
          message.error(error.message || '排序更新失败')
          // 失败时重新获取数据以恢复原状
          fetchData()
        }
      }
    }
  })
}

// 监听数据变化，确保在数据加载后重新初始化
watch(() => links.value, () => {
  nextTick(() => {
    initSortable()
  })
}, { deep: false })

onMounted(() => {
  fetchData()
})
</script>

<style scoped>
.custom-table :deep(.n-data-table-th) {
  background-color: transparent;
  border-bottom: 1px solid var(--border-primary);
  font-weight: 600;
}

.custom-table :deep(.n-data-table-td) {
  background-color: transparent;
  border-bottom: 1px solid var(--border-primary);
}

.custom-table :deep(.n-data-table-tr:last-child .n-data-table-td) {
  border-bottom: none;
}

.custom-table :deep(.n-data-table-tr:hover .n-data-table-td) {
  background-color: var(--hover-bg);
}

/* Naive UI Input Customization for Apple Style */
:deep(.n-input) {
  background-color: transparent !important;
}

:deep(.n-input .n-input__border),
:deep(.n-input .n-input__state-border) {
  border: 1px solid var(--border-primary) !important;
  border-radius: 0.75rem !important;
  transition: all 0.2s ease;
}

:deep(.n-input:hover .n-input__state-border) {
  border-color: var(--text-tertiary) !important;
}

:deep(.n-input.n-input--focus .n-input__state-border) {
  border-color: #3b82f6 !important;
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.2) !important;
}

.animate-fade-in {
  animation: fadeIn 0.4s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>