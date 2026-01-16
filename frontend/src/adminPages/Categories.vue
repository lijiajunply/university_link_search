<template>
  <div class="space-y-6 animate-fade-in">
    <!-- 顶部标题栏 -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div>
        <h1 class="text-2xl sm:text-3xl font-bold text-[--text-primary] tracking-tight">分类管理</h1>
        <p class="mt-1 text-sm text-[--text-secondary]">管理网站首页显示的分类及其排序</p>
      </div>
      <button
        @click="handleCreate"
        class="flex items-center justify-center gap-2 px-4 py-2.5 bg-blue-500 hover:bg-blue-600 active:bg-blue-700 text-white rounded-xl font-medium transition-all duration-200 shadow-lg shadow-blue-500/20 hover:shadow-blue-500/40 active:scale-95">
        <Icon icon="solar:add-circle-bold" class="w-5 h-5" />
        <span>新建分类</span>
      </button>
    </div>

    <!-- 数据表格区域 -->
    <div class="bg-white/80 dark:bg-[#18181c]/80 backdrop-blur-xl rounded-2xl border border-[--border-primary] shadow-sm overflow-hidden">
      <n-data-table
        :columns="columns"
        :data="categories"
        :loading="loading"
        :bordered="false"
        :single-line="false"
        class="custom-table"
      />
    </div>

    <!-- 编辑/创建弹窗 -->
    <n-modal
      v-model:show="showModal"
      :mask-closable="false"
      transform-origin="center">
      <div class="w-full max-w-lg bg-white dark:bg-[#1f1f23] rounded-2xl shadow-2xl border border-[--border-primary] overflow-hidden">
        <!-- 弹窗标题 -->
        <div class="px-6 py-4 border-b border-[--border-primary] flex items-center justify-between bg-[--bg-secondary]/50">
          <h3 class="text-lg font-bold text-[--text-primary]">
            {{ isEdit ? '编辑分类' : '新建分类' }}
          </h3>
          <button 
            @click="showModal = false"
            class="p-1 rounded-lg hover:bg-[--hover-bg] text-[--text-tertiary] hover:text-[--text-primary] transition-colors">
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
                    <Icon icon="solar:text-bold" class="text-[--text-tertiary]" />
                  </template>
                </n-input>
              </n-form-item>

              <n-form-item label="唯一标识 (Key)" path="key">
                <n-input v-model:value="formModel.key" placeholder="例如：study" class="custom-input">
                  <template #prefix>
                    <Icon icon="solar:key-bold" class="text-[--text-tertiary]" />
                  </template>
                </n-input>
              </n-form-item>
            </div>

            <n-form-item label="图标 (Iconify)" path="icon">
              <div class="flex gap-3 w-full">
                <div class="flex-1">
                  <n-input v-model:value="formModel.icon" placeholder="例如：solar:book-bold" class="custom-input">
                    <template #prefix>
                      <Icon icon="solar:sticker-smile-circle-2-bold" class="text-[--text-tertiary]" />
                    </template>
                  </n-input>
                </div>
                <div class="flex-shrink-0 w-10 h-10 rounded-xl bg-[--bg-secondary] border border-[--border-primary] flex items-center justify-center">
                  <Icon v-if="formModel.icon" :icon="formModel.icon" class="w-6 h-6 text-[--text-primary]" />
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
        <div class="px-6 py-4 bg-[--bg-secondary]/30 flex justify-end gap-3">
          <button
            @click="showModal = false"
            class="px-4 py-2 rounded-xl text-sm font-medium text-[--text-secondary] hover:bg-[--hover-bg] transition-colors">
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
import { h, onMounted, ref, reactive } from 'vue'
import { 
  NDataTable, NButton, NModal, NForm, NFormItem, NInput, NInputNumber, 
  useMessage, useDialog, type DataTableColumns, type FormInst
} from 'naive-ui'
import { Icon } from '@iconify/vue'
import { useRouter } from 'vue-router'
import { CategoryService } from '../services/CategoryService'
import type { CategoryModel } from '../models/category'

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
    title: '图标',
    key: 'icon',
    width: 80,
    render(row) {
      return h('div', { class: 'w-10 h-10 rounded-xl bg-blue-500/10 flex items-center justify-center' }, [
        h(Icon, { icon: row.icon, class: 'w-6 h-6 text-blue-500' })
      ])
    }
  },
  {
    title: '分类名称',
    key: 'name',
    render(row) {
      return h('div', { class: 'flex flex-col' }, [
        h('span', { class: 'font-semibold text-[--text-primary]' }, row.name),
        h('span', { class: 'text-xs text-[--text-tertiary]' }, row.key)
      ])
    }
  },
  {
    title: '描述',
    key: 'description',
    render(row) {
      return h('span', { class: 'text-[--text-secondary] text-sm' }, row.description || '-')
    }
  },
  {
    title: '排序',
    key: 'index',
    width: 80,
    render(row) {
      return h('div', { class: 'font-mono text-[--text-secondary]' }, row.index)
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
          // 检查 Service: fetch(`${url}/category/${id}`). 通常 ID 是 _id. 
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

onMounted(() => {
  fetchData()
})
</script>

<style scoped>
.custom-table :deep(.n-data-table-th) {
  background-color: transparent;
  border-bottom: 1px solid var(--border-primary);
  color: var(--text-secondary);
  font-weight: 600;
}

.custom-table :deep(.n-data-table-td) {
  background-color: transparent;
  border-bottom: 1px solid var(--border-primary);
  color: var(--text-primary);
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