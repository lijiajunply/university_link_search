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
        <div class="relative flex-grow">
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
                  <div class="flex-shrink-0 h-10 w-10">
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
                  @click="toggleUserStatus(user)" 
                  class="text-info hover:text-info/80 mr-3 transition-colors"
                  :disabled="isCurrentUser(user.userId)"
                  :title="isCurrentUser(user.userId) ? '不能修改当前用户状态' : ''"
                >
                  <Icon icon="ep:switch-button" class="h-4 w-4" :class="isCurrentUser(user.userId) ? 'opacity-50' : ''" />
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
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">姓名</label>
          <input
            v-model="form.name"
            type="text"
            placeholder="请输入姓名"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          />
        </div>

        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">邮箱</label>
          <input
            v-model="form.email"
            type="email"
            placeholder="请输入邮箱"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          />
        </div>

        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">手机号</label>
          <input
            v-model="form.phone"
            type="tel"
            placeholder="请输入手机号"
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

        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">状态 <span class="text-danger">*</span></label>
          <select
            v-model="form.status"
            :disabled="!!isCurrentUserEditing"
            class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
          >
            <option value="active">启用</option>
            <option value="inactive">禁用</option>
          </select>
          <p v-if="isCurrentUserEditing" class="text-xs text-gray-500 dark:text-gray-400">不能修改当前用户的状态</p>
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
import type { UserModel } from '../models/user'

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

// 表单数据
const form = reactive({
  username: '',
  password: '',
  name: '',
  email: '',
  phone: '',
  role: 'user' as 'user' | 'admin',
  status: 'active' as 'active' | 'inactive'
})

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
    users.value = await UserService.getAllUsers()
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
  form.name = ''
  form.email = ''
  form.phone = ''
  form.role = 'user'
  form.status = 'active'
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

const selectedUser = ref<UserModel | null>(null)

const editUser = (user: UserModel) => {
  editingUser.value = true
  showDialog.value = true
  selectedUser.value = user
  editingId.value = user.userId
  form.username = user.userName
  form.password = '' // 编辑模式下密码为空
  form.name = (user as any).name || ''
  form.email = (user as any).email || ''
  form.phone = (user as any).phone || ''
  form.role = (user.identity || 'user') as 'user' | 'admin'
  form.status = ((user as any).status || 'active') as 'active' | 'inactive'
}

const saveUser = async () => {
  if (!form.username) {
    message.warning('请填写用户名')
    return
  }
  
  if (!editingUser && !form.password) {
    message.warning('请填写密码')
    return
  }
  
  try {
    saving.value = true
    
    // 准备提交数据，编辑模式下如果密码为空则不更新密码
      let submitData = { ...form }
        if (editingUser && !submitData.password) {
          // 不使用delete操作符，而是创建新对象
          const { password, ...formWithoutPassword } = submitData
          submitData = formWithoutPassword as any
        }

      if (editingId.value) {
        // 编辑模式
        const updateData = {
          userId: editingId.value,
          userName: submitData.username,
          eMail: submitData.email,
          gender: 'male',
          identity: submitData.role,
          className: '1-1',
        } as UserModel
        if (submitData.password) {
          (updateData as any).Password = submitData.password
        }
        await UserService.updateUser(editingId.value!, updateData)
        message.success('更新成功')
      } else {
        // 创建模式
        const createData: any = {
          UserName: submitData.username,
          Password: submitData.password,
          Name: submitData.name,
          Email: submitData.email,
          Phone: submitData.phone,
          Role: submitData.role,
          Status: submitData.status
        }
        await UserService.createUser(createData)
        message.success('创建成功')
      }
    
    await loadUsers()
    closeDialog()
  } catch (error) {
    message.error(editingId.value ? '更新失败' : '创建失败')
  } finally {
    saving.value = false
  }
}

const toggleUserStatus = async (user: UserModel) => {
  if (isCurrentUser(user.userId)) {
    message.warning('不能修改当前用户的状态')
    return
  }
  
  const newStatus = (user as any).status === 'active' ? 'inactive' : 'active'
  const statusText = newStatus === 'active' ? '启用' : '禁用'
  
  if (confirm(`确定要${statusText}用户「${user.userName}」吗？`)) {
    try {
      await UserService.updateUser(user.userId, {
        userId: user.userId,
        userName: user.userName,
        gender: user.gender,
        className: user.className,
        identity: user.identity,
      })
      message.success(`${statusText}成功`)
      await loadUsers()
    } catch (error) {
      message.error(`${statusText}失败`)
    }
  }
}

const deleteUser = async (id: string) => {
  if (isCurrentUser(id)) {
    message.warning('不能删除当前用户')
    return
  }
  
  const user = users.value.find((u: UserModel) => u.userId === id)
  if (!user) return
  
  if (confirm(`确定要删除用户「${user.userName}」吗？此操作不可恢复。`)) {
    try {
      await UserService.deleteUser(id)
      message.success('删除成功')
      await loadUsers()
    } catch (error) {
      message.error('删除失败')
    }
  }
}

const prevPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

const nextPage = () => {
  if (endIndex.value < filteredUsers.value.length) {
    currentPage.value++
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