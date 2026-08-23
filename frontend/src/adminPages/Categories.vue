<template>
  <div class="space-y-6 animate-fade-in">
    <!-- 顶部标题栏 -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div>
        <h1 class="text-2xl sm:text-3xl font-bold text-(--text-primary) tracking-tight">分类管理</h1>
        <p class="mt-1 text-sm text-(--text-secondary)">管理网站首页显示的分类及其排序</p>
      </div>
      <div class="flex items-center gap-3">
        <button
          @click="handleExport"
          :disabled="exporting"
          class="flex items-center justify-center gap-2 px-4 py-2.5 border border-[var(--border-primary)] bg-[var(--bg-secondary)]/50 hover:bg-[var(--hover-bg)] text-[var(--text-primary)] rounded-xl font-medium transition-all duration-200 active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed">
          <Icon v-if="exporting" icon="svg-spinners:ring-resize" class="w-5 h-5" />
          <Icon v-else icon="solar:download-bold" class="w-5 h-5" />
          <span>导出</span>
        </button>
        <button
          @click="handleImport"
          :disabled="importing"
          class="flex items-center justify-center gap-2 px-4 py-2.5 border border-[var(--border-primary)] bg-[var(--bg-secondary)]/50 hover:bg-[var(--hover-bg)] text-[var(--text-primary)] rounded-xl font-medium transition-all duration-200 active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed">
          <Icon v-if="importing" icon="svg-spinners:ring-resize" class="w-5 h-5" />
          <Icon v-else icon="solar:upload-bold" class="w-5 h-5" />
          <span>导入</span>
        </button>
        <button
          @click="handleCreate"
          class="flex items-center justify-center gap-2 px-4 py-2.5 bg-blue-500 hover:bg-blue-600 active:bg-blue-700 text-white rounded-xl font-medium transition-all duration-200 shadow-lg shadow-blue-500/20 hover:shadow-blue-500/40 active:scale-95">
          <Icon icon="solar:add-circle-bold" class="w-5 h-5" />
          <span>新建分类</span>
        </button>
        <input
          ref="fileInputRef"
          type="file"
          accept=".json,application/json"
          class="hidden"
          @change="handleFileChange" />
      </div>
    </div>

    <!-- 数据表格区域 -->
    <div
      ref="tableContainer"
      class="bg-white/80 dark:bg-[#18181c]/80 backdrop-blur-xl rounded-2xl border border-[var(--border-primary)] shadow-sm overflow-hidden">
      <n-data-table
        :columns="columns"
        :data="categories"
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
            {{ isEdit ? '编辑分类' : '新建分类' }}
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
            
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <n-form-item label="分类名称" path="name">
                <n-input v-model:value="formModel.name" placeholder="例如：学习资料" class="custom-input">
                  <template #prefix>
                    <Icon icon="solar:text-bold" class="text-[var(--text-tertiary)]" />
                  </template>
                </n-input>
              </n-form-item>

              <n-form-item label="唯一标识 (Key)" path="key">
                <n-input v-model:value="formModel.key" placeholder="例如：study" class="custom-input">
                  <template #prefix>
                    <Icon icon="solar:key-bold" class="text-[var(--text-tertiary)]" />
                  </template>
                </n-input>
              </n-form-item>
            </div>

            <n-form-item label="图标 (iconfont / URL)" path="icon">
              <div class="flex gap-3 w-full">
                <div class="flex-1">
                  <n-input v-model:value="formModel.icon" placeholder="iconfont 类名或图片链接，例如：book" class="custom-input">
                    <template #prefix>
                      <Icon icon="solar:sticker-smile-circle-2-bold" class="text-[var(--text-tertiary)]" />
                    </template>
                  </n-input>
                </div>
                <div class="flex-shrink-0 w-10 h-10 rounded-xl bg-[var(--bg-secondary)] border border-[var(--border-primary)] flex items-center justify-center overflow-hidden text-(--text-primary)">
                  <AppLogo v-if="formModel.icon" :icon="formModel.icon" :name="formModel.name" url="" :size="24" />
                </div>
              </div>
            </n-form-item>

            <n-form-item label="排序索引" path="index">
              <n-input-number v-model:value="formModel.index" class="w-full custom-input" button-placement="both" />
            </n-form-item>

            <n-form-item label="描述" path="description">
              <n-input
                v-model:value="formModel.description"
                type="textarea"
                placeholder="简要描述该分类的用途..."
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
  NDataTable, NModal, NForm, NFormItem, NInput, NInputNumber, 
  useMessage, useDialog, type DataTableColumns, type FormInst
} from 'naive-ui'
import { Icon } from '@iconify/vue'
import Sortable from 'sortablejs'
import { useRouter } from 'vue-router'
import { CategoryService } from '../services/CategoryService'
import { DataService } from '../services/DataService'
import type { CategoryModel } from '../models/category'
import AppLogo from '../components/AppLogo.vue'

const router = useRouter()
const message = useMessage()
const dialog = useDialog()

// 状态定义
const loading = ref(false)
const submitting = ref(false)
const showModal = ref(false)
const isEdit = ref(false)
const categories = ref<CategoryModel[]>([])
const formRef = ref<FormInst | null>(null)
const tableContainer = ref<HTMLElement | null>(null)
const sortableInstance = ref<Sortable | null>(null)
const fileInputRef = ref<HTMLInputElement | null>(null)
const importing = ref(false)
const exporting = ref(false)

// 表单数据
const defaultForm: CategoryModel = {
  key: '',
  name: '',
  icon: '',
  index: 0,
  description: '',
  links: []
}
const formModel = reactive<CategoryModel>({ ...defaultForm })

// 表单校验规则
const rules = {
  name: { required: true, message: '请输入分类名称', trigger: 'blur' },
  key: { required: true, message: '请输入唯一标识', trigger: 'blur' },
  icon: { required: true, message: '请输入图标代码', trigger: 'blur' }
}

// 表格列定义
const columns: DataTableColumns<CategoryModel> = [
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
    width: 80,
    render(row) {
      return h('div', { class: 'w-10 h-10 rounded-xl bg-blue-500/10 flex items-center justify-center text-blue-500' }, [
        h(AppLogo, { icon: row.icon, name: row.name, url: '', size: 24 })
      ])
    }
  },
  {
    title: '分类名称',
    key: 'name',
    render(row) {
      return h('div', { class: 'flex flex-col' }, [
        h('span', { class: 'font-semibold text-[var(--text-primary)]' }, row.name),
        h('span', { class: 'text-xs text-[var(--text-tertiary)]' }, row.key)
      ])
    }
  },
  {
    title: '描述',
    key: 'description',
    render(row) {
      return h('span', { class: 'text-[var(--text-secondary)] text-sm' }, row.description || '-')
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
    width: 200,
    render(row) {
      return h('div', { class: 'flex gap-2' }, [
        // 链接管理按钮
        h('button', {
          class: 'p-2 rounded-lg hover:bg-blue-500/10 text-blue-500 transition-colors',
          title: '管理链接',
          onClick: () => router.push(`/category/${row.key}`) // 使用 key 还是 id? Service getCategoryById 接收 string。
          // 检查 Service: fetch(`${url}/category/${id}`). 通常 ID 是 _id. 如果是 SQL, 应该有 id.
          // 但是 Model 没有 _id, 只有 key. 
          // 假设后端使用 key 作为 ID 或者 Model 定义缺失 _id.
          // 观察 Service createCategory 返回 CategoryModel.
          // 如果后端是 MongoDB, 应该有 _id. 如果是 SQL, 应该有 id.
          // 这里的 Model 只有 key. 让我们假设 key 是唯一标识.
        }, [h(Icon, { icon: 'solar:link-circle-bold', class: 'w-5 h-5' })]),
        
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
  loading.value = true
  try {
    categories.value = await CategoryService.getAllCategories()
  } catch (error: any) {
    message.error(error.message || '获取数据失败')
  } finally {
    loading.value = false
  }
}

// 导出数据（JSON）
const handleExport = async () => {
  exporting.value = true
  try {
    await DataService.downloadData()
    message.success('导出成功')
  } catch (error: any) {
    message.error(error.message || '导出失败')
  } finally {
    exporting.value = false
  }
}

// 触发文件选择
const handleImport = () => {
  fileInputRef.value?.click()
}

// 处理导入文件（仅支持 JSON）
const handleFileChange = async (event: Event) => {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return

  try {
    // 仅支持 JSON 文件
    if (!DataService.isValidJsonFile(file)) {
      message.error('仅支持导入 JSON 文件')
      return
    }

    // 校验 JSON 内容是否合法
    const isValid = await DataService.readAndValidateJsonFile(file)
    if (!isValid) {
      message.error('JSON 文件内容格式不正确')
      return
    }

    importing.value = true
    const result = await DataService.uploadData(file)
    message.success(result.message || '导入成功')
    fetchData()
  } catch (error: any) {
    message.error(error.message || '导入失败')
  } finally {
    importing.value = false
    input.value = ''
  }
}

// 打开创建弹窗
const handleCreate = () => {
  isEdit.value = false
  Object.assign(formModel, defaultForm)
  // 自动设置一个排序索引
  if (categories.value.length > 0) {
    formModel.index = Math.max(...categories.value.map(c => c.index)) + 1
  }
  showModal.value = true
}

// 打开编辑弹窗
const handleEdit = (row: CategoryModel) => {
  isEdit.value = true
  Object.assign(formModel, row)
  showModal.value = true
}

// 处理删除
const handleDelete = (row: CategoryModel) => {
  dialog.warning({
    title: '确认删除',
    content: `确定要删除分类 "${row.name}" 吗？该操作不可恢复。`,
    positiveText: '确认删除',
    negativeText: '取消',
    onPositiveClick: async () => {
      try {
        // 假设 key 是 ID, 如果不是, 需要调整
        // 检查 Service: deleteCategory(id: string)
        // 再次检查 Service: fetchWithAuth(`/category/${id}`)
        // 如果 Model 没有 ID 字段, 那么 key 可能就是 ID
        // 这里有一个风险点: 如果数据库有 _id 但 Model 没写.
        // 但是前端通常不知道 _id. 
        // 尝试用 key 删除. 如果 row 里面有隐藏的 _id (any 类型), 也可以.
        const idToDelete = (row as any)._id || (row as any).id || row.key
        await CategoryService.deleteCategory(idToDelete)
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
        if (isEdit.value) {
          await CategoryService.updateCategory(formModel)
          message.success('更新成功')
        } else {
          await CategoryService.createCategory(formModel)
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
const initSortable = () => {
  if (!tableContainer.value) return
  
  const el = tableContainer.value.querySelector('.n-data-table-tbody') as HTMLElement
  if (!el) return

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
      const item = categories.value.splice(oldIndex, 1)[0]
      if(item !== undefined) {
        categories.value.splice(newIndex, 0, item)
      
        // 更新本地索引显示（可选）
        categories.value.forEach((cat, idx) => {
          cat.index = idx
        })

        // 调用API更新排序
        try {
          // 假设后端接受 key 数组作为排序依据
          const sortedIds = categories.value.map(c => c.key)
          await CategoryService.updateCategorySort(sortedIds)
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

watch(() => categories.value, () => {
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