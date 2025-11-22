<template>
  <div class="py-6 px-4 sm:px-6 lg:px-8">
    <!-- 页面标题 -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6">
      <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">用户管理</h1>
      <button 
        @click="showCreateDialog = true" 
        class="mt-4 sm:mt-0 inline-flex items-center px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all"
      >
        <Icon icon="ep:plus" class="mr-2 h-4 w-4" />
        添加用户
      </button>
    </div>

    <!-- 搜索和筛选 -->
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-4 mb-6">
      <div class="flex flex-col lg:flex-row gap-4">
        <div class="relative grow">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Icon icon="ep:search" class="h-5 w-5 text-gray-400" />
          </div>
          <input
            v-model="searchQuery"
            type="text"
            class="pl-10 block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
            placeholder="搜索用户名、邮箱或手机号"
          />
        </div>
        <div class="flex gap-2">
          <select
            v-model="filterRole"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          >
            <option value="">所有角色</option>
            <option value="admin">管理员</option>
            <option value="user">普通用户</option>
          </select>
          <button 
            @click="loadUsers" 
            class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg shadow-sm text-sm font-medium text-gray-700 dark:text-gray-200 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all"
          >
            <Icon icon="ep:refresh" class="h-4 w-4 mr-2" />
            刷新
          </button>
        </div>
      </div>
    </div>

    <!-- 用户列表 -->
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                头像
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                用户名
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                邮箱
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                手机号
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                角色
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                状态
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
            <tr v-for="user in paginatedUsers" :key="user.userId" class="hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="shrink-0 h-10 w-10">
                    <img 
                      :src="getAvatarUrl(user)" 
                      alt="用户头像" 
                      class="h-10 w-10 rounded-full object-cover border-2 border-white dark:border-gray-700 shadow-sm"
                    />
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900 dark:text-white">{{ user.userName }}</div>
    <div class="text-sm text-gray-500 dark:text-gray-400">{{ (user as any).name || '未设置姓名' }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ (user as any).email || '未设置邮箱' }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ (user as any).phone || '未设置手机号' }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full" 
                  :class="user.identity === 'admin' 
                    ? 'bg-purple-100 dark:bg-purple-900 text-purple-800 dark:text-purple-200' 
                    : 'bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200'"
                >
                  {{ user.identity === 'admin' ? '管理员' : '普通用户' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full" 
                  :class="(user as any).status === 'active' 
                    ? 'bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200' 
                    : 'bg-red-100 dark:bg-red-900 text-red-800 dark:text-red-200'"
                >
                  {{ (user as any).status === 'active' ? '启用' : '禁用' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-500 dark:text-gray-400">{{ (user as any).createTime ? formatDate((user as any).createTime) : '未知' }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button 
                  @click="editUser(user)" 
                  class="text-primary hover:text-primary/80 mr-3 transition-colors"
                  :disabled="isCurrentUser(user.userId)"
                  :title="isCurrentUser(user.userId) ? '不能编辑当前用户' : ''"
                >
                  <Icon icon="ep:edit" class="h-4 w-4" :class="isCurrentUser(user.userId) ? 'opacity-50' : ''" />
                </button>
                <button 
                  @click="deleteUser(user.userId)" 
                  class="text-danger hover:text-danger/80 transition-colors"
                  :disabled="isCurrentUser(user.userId)"
                  :title="isCurrentUser(user.userId) ? '不能删除当前用户' : ''"
                >
                  <Icon icon="ep:delete" class="h-4 w-4" :class="isCurrentUser(user.userId) ? 'opacity-50' : ''" />
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- 空状态 -->
      <div v-if="users.length === 0 && !loading" class="p-8 text-center">
        <Icon icon="ep:user" class="h-12 w-12 text-gray-400 mx-auto mb-4" />
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-1">暂无用户</h3>
        <p class="text-gray-500 dark:text-gray-400">点击"添加用户"按钮创建新用户</p>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="p-8 text-center">
        <n-spin size="large" />
        <p class="mt-4 text-gray-500 dark:text-gray-400">加载中...</p>
      </div>
    </div>

    <!-- 分页 -->
    <div v-if="users.length > 0 && !loading" class="mt-4 flex justify-between items-center">
      <p class="text-sm text-gray-500 dark:text-gray-400">
        显示 {{ startIndex + 1 }} 到 {{ Math.min(endIndex, users.length) }} 条，共 {{ users.length }} 条
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
          :disabled="endIndex >= users.length"
          class="px-3 py-1 rounded-lg text-sm font-medium transition-all"
          :class="endIndex >= users.length ? 'text-gray-400 cursor-not-allowed' : 'text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700'"
        >
          下一页
        </button>
      </div>
    </div>

    <n-modal
      v-model:show="showDialog as any"
      :title="editingUser ? '编辑用户' : '创建用户'"
      preset="dialog"
      size="large"
    >
      <div class="space-y-4">
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">用户名 <span class="text-danger">*</span></label>
          <input
            v-model="form.username"
            type="text"
            :disabled="editingUser"
            placeholder="请输入用户名"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          />
          <p v-if="editingUser" class="text-xs text-gray-500 dark:text-gray-400">用户名创建后不可修改</p>
        </div>
        
        <div v-if="!editingUser" class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">密码 <span class="text-danger">*</span></label>
          <input
            v-model="form.password"
            type="password"
            placeholder="请输入密码"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          />
        </div>
        
        <div v-if="editingUser" class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">重置密码</label>
          <input
            v-model="form.password"
            type="password"
            placeholder="留空表示不修改密码"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          />
        </div>

        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">角色 <span class="text-danger">*</span></label>
          <select
            v-model="form.role"
            :disabled="!!isCurrentUserEditing"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          >
            <option value="user">普通用户</option>
            <option value="admin">管理员</option>
          </select>
          <p v-if="isCurrentUserEditing" class="text-xs text-gray-500 dark:text-gray-400">不能修改当前用户的角色</p>
        </div>
      </div>
      <template #footer>
        <button 
          @click="closeDialog" 
          class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg shadow-sm text-sm font-medium text-gray-700 dark:text-gray-200 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all mr-2"
        >
          取消
        </button>
        <button 
          @click="saveUser" 
          :disabled="saving || !form.username || (!editingUser && !form.password)"
          class="px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all disabled:opacity-70 disabled:cursor-not-allowed"
        >
          {{ saving ? '保存中...' : '保存' }}
        </button>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted } from 'vue'
import { useMessage } from 'naive-ui'
import { NSpin, NModal } from 'naive-ui'
import { Icon } from '@iconify/vue'
import { UserService } from '../services/UserService'
import {type UserModel} from '../models/user'

// 消息提示
const message = useMessage()

// 状态
const loading = ref(false)
const saving = ref(false)
const showDialog = ref(false)
const showCreateDialog = ref(false)
const showDeleteConfirm = ref(false)
const editingUser = ref(false)
const editingId = ref<string | null>(null)

// 分页
const currentPage = ref(1)
const pageSize = 10

// 搜索和筛选
const searchQuery = ref('')
const filterRole = ref('')

// 数据
const users = ref<UserModel[]>([])
const currentUserId = ref<string | null>(null) // 可以从本地存储或全局状态获取
const selectedUser = ref<UserModel | null>(null)

// 表单数据
const form = reactive({
  username: '',
  password: '',
  role: 'user' as 'admin' | 'user',
  className: ''
})

const needUpdatePassword = ref(false)

// 计算属性
const filteredUsers = computed(() => {
  let result = [...users.value]
  
  // 按搜索词过滤
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(user => 
      user.userName.toLowerCase().includes(query)
    )
  }
  
  // 按角色过滤
  if (filterRole.value) {
    result = result.filter(user => user.identity === filterRole.value)
  }
  
  return result
})

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * pageSize
  const end = start + pageSize
  return filteredUsers.value.slice(start, end)
})

const startIndex = computed(() => (currentPage.value - 1) * pageSize)
const endIndex = computed(() => startIndex.value + pageSize)

const isCurrentUserEditing = computed(() => {
  return currentUserId.value && editingId.value === currentUserId.value
})

// 方法
const loadUsers = async () => {
  try {
    loading.value = true
    const data = await UserService.getAllUsers()
    // 标准化用户数据格式，确保使用正确的属性名和类型
    users.value = data.map((item: any) => ({
      userId: item.userId,
      userName: item.userName,
      name: item.name || '',
      email: item.email || '',
      phone: item.phone || '',
      identity: item.identity || 'user',
      active: item.active !== undefined ? item.active : (item.status === 'active'), // 兼容status字段
      createTime: item.createTime,
      gender: item.gender || '',
      className: item.className || ''
    }))
    
    // 假设获取当前用户信息
    const currentUser = await UserService.getCurrentUser()
    if (currentUser) {
      currentUserId.value = currentUser.userId
    }
  } catch (error) {
      message.error('加载用户失败')
    } finally {
    loading.value = false
  }
}

const resetForm = () => {
  form.username = ''
  form.password = ''
  form.role = 'user'
  editingId.value = null
}

const closeDialog = () => {
  showDialog.value = false
  showCreateDialog.value = false
  showDeleteConfirm.value = false
  editingUser.value = false
  selectedUser.value = null
  resetForm()
}

const editUser = (user: UserModel) => {
  editingUser.value = true
  showDialog.value = true
  selectedUser.value = user
  editingId.value = user.userId
  form.username = user.userName
  form.password = '' // 清空密码字段，编辑时默认不修改密码
  form.role = user.identity as 'admin' | 'user'
  needUpdatePassword.value = false
}

const validateForm = (): boolean => {
  if (!form.username || form.username.trim() === '') {
    message.warning('请输入用户名')
    return false
  }
  
  // 新增用户或修改密码时验证密码
  if (!editingUser.value || form.password) {
    if (!form.password || form.password.length < 6) {
      message.warning('密码长度不能少于6位')
      return false
    }
    
    // 密码强度检查
    const passwordStrength = checkPasswordStrength(form.password)
    if (passwordStrength < 2) {
      message.warning('密码强度太弱，请包含字母和数字')
      return false
    }
  }
  
  return true
}

// 密码强度检查
const checkPasswordStrength = (password: string): number => {
  let strength = 0
  if (/[a-zA-Z]/.test(password)) strength++
  if (/[0-9]/.test(password)) strength++
  if (/[^a-zA-Z0-9]/.test(password)) strength++
  if (password.length >= 8) strength++
  return strength
}

const saveUser = async () => {
  try {
    // 表单验证
    if (!validateForm()) {
      return
    }
    
    saving.value = true
    
    // 准备用户数据，使用正确的类型和字段名
    const userData: Partial<UserModel> = {
      userName: form.username,
      identity: form.role,
    }
    
    if (editingId.value) {
      // 编辑模式
      const baseData: UserModel = {
        userId: editingId.value,
        userName: form.username,
        identity: form.role,
        className: form.className,
      }
      
      await UserService.updateUser(editingId.value, baseData)
      
      // 如果提供了新密码，则更新密码
      if (form.password) {
        await UserService.updatePassword(editingId.value, form.password)
      }
      
      message.success('更新成功')
    } else {
      // 创建模式
      if (!form.password) {
        throw new Error('创建用户必须提供密码')
      }
      
      const createData: UserModel = {
        ...userData as UserModel,
        userId: '', // 将由后端生成
      }
      
      await UserService.createUser(createData)
      message.success('创建成功')
    }
    
    await loadUsers()
    closeDialog()
  } catch (error: any) {
    console.error('保存用户失败:', error)
    message.error(editingId.value ? '更新失败: ' + (error.message || '') : '创建失败: ' + (error.message || ''))
  } finally {
    saving.value = false
  }
}


const deleteUser = async (id: string) => {
  if (isCurrentUser(id)) {
    message.warning('不能删除当前用户')
    return
  }
  
  const user = users.value.find((u: UserModel) => u.userId === id)
  if (!user) return
  
  // 确认对话框
  if (!confirm(`确定要删除用户「${user.userName}」吗？此操作不可撤销。`)) {
    return
  }
  
  try {
    await UserService.deleteUser(id)
    message.success('删除成功')
    await loadUsers()
  } catch (error: any) {
    console.error('删除用户失败:', error)
    message.error('删除失败: ' + (error.message || '请重试'))
  }
}

const prevPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  } else {
    message.info('已经是第一页了')
  }
}

const nextPage = () => {
  const totalPages = Math.ceil(filteredUsers.value.length / pageSize)
  if (currentPage.value < totalPages) {
    currentPage.value++
  } else {
    message.info('已经是最后一页了')
  }
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

const getAvatarUrl = (user: UserModel): string => {
  // 如果有头像URL则使用，否则生成基于用户名的默认头像
  if ((user as any).avatar) {
    return (user as any).avatar
  }
  
  // 使用ui-avatars服务生成默认头像
  return `https://ui-avatars.com/api/?name=${encodeURIComponent(user.userName || '用户')}&background=random&color=fff&size=128`
}

const isCurrentUser = (userId: string): boolean => {
  return currentUserId.value === userId
}

// 生命周期
onMounted(async () => {
  await loadUsers()
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

/* 自定义按钮禁用样式 */
button:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}
</style>