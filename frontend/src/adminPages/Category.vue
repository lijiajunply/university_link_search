<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 p-4 md:p-6 transition-colors duration-300">
    <div class="max-w-6xl mx-auto">
      <!-- 页面标题 -->
      <div class="mb-6 flex items-center justify-between">
        <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">分类详情</h1>
        <button @click="goBack" class="flex items-center px-4 py-2 rounded-lg bg-gray-100 dark:bg-gray-800 hover:bg-gray-200 dark:hover:bg-gray-700 transition-colors">
          <ArrowLeftIcon class="w-5 h-5 mr-2" />
          <span class="text-gray-700 dark:text-gray-300">返回列表</span>
        </button>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="flex justify-center items-center py-12">
        <n-spin size="large" :stroke-width="2" />
      </div>

      <!-- 错误状态 -->
      <div v-else-if="error" class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4 mb-6">
        <p class="text-red-600 dark:text-red-400">{{ error }}</p>
      </div>

      <!-- 分类详情 -->
      <div v-else-if="category" class="space-y-6">
        <!-- 分类基本信息 -->
        <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6 transition-all duration-300 hover:shadow-md">
          <h2 class="text-xl font-medium text-gray-900 dark:text-white mb-4">基本信息</h2>
          
          <div class="space-y-4">
            <div class="flex items-center mb-4">
              <div v-if="category?.icon" class="w-16 h-16 flex items-center justify-center rounded-xl bg-gray-100 dark:bg-gray-700 mr-4">
                <IconFont :type="category.icon" class="h-8 w-8 text-primary" />
              </div>
              <div v-else class="w-16 h-16 flex items-center justify-center rounded-xl bg-gray-100 dark:bg-gray-700 mr-4">
                <Icon icon="ep:folder" class="h-8 w-8 text-primary" />
              </div>
            </div>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div class="space-y-2">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">名称</label>
                <n-input 
                  v-model:value="editingCategory.name" 
                  placeholder="输入分类名称"
                  :disabled="!isEditing"
                  class="w-full"
                />
              </div>
              
              <div class="space-y-2">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">图标</label>
                <n-input 
                  v-model:value="editingCategory.icon" 
                  placeholder="输入图标名称"
                  :disabled="!isEditing"
                  class="w-full"
                />
              </div>
            </div>
            
            <div class="space-y-2">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">描述</label>
              <n-input 
                v-model:value="editingCategory.description" 
                type="textarea"
                placeholder="输入分类描述"
                :disabled="!isEditing"
                :rows="3"
                class="w-full"
              />
            </div>

            <!-- 操作按钮 -->
            <div class="flex justify-end pt-2 space-x-3">
              <n-button 
                v-if="isEditing" 
                @click="cancelEdit" 
                text
              >
                取消
              </n-button>
              <n-button 
                v-if="isEditing" 
                @click="saveCategoryInfo" 
                type="primary"
                :loading="savingInfo"
              >
                保存
              </n-button>
              <n-button 
                v-else 
                @click="startEdit" 
                type="primary"
              >
                编辑
              </n-button>
            </div>
          </div>
        </div>

        <!-- 链接列表 -->
        <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6 transition-all duration-300 hover:shadow-md">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-xl font-medium text-gray-900 dark:text-white">链接管理</h2>
            <n-button @click="showAddLinkModal = true" type="primary" size="small" class="flex items-center">
              <PlusIcon class="w-4 h-4 mr-1" />
              添加链接
            </n-button>
          </div>

          <!-- 拖拽提示 -->
          <div class="mb-4 text-sm text-gray-500 dark:text-gray-400 bg-gray-50 dark:bg-gray-750 p-3 rounded-lg">
            <DragDropIcon class="w-4 h-4 inline mr-2" />
            拖拽链接可以调整顺序
          </div>

          <!-- 链接列表 -->
          <div class="space-y-3" id="link-list">
            <div 
              v-for="link in category.links" 
              :key="link.key"
              :draggable="true"
              @dragstart="handleDragStart($event, link)"
              @dragenter="handleDragEnter($event, link)"
              @dragleave="handleDragLeave"
              @dragover.prevent
              @drop="handleDrop($event, link)"
              :class="['p-4 rounded-lg border transition-all duration-200', 
                isDragging && draggedLink?.key === link.key ? 
                'opacity-50 border-blue-400 dark:border-blue-500 bg-blue-50 dark:bg-blue-900/20' : 
                'border-gray-200 dark:border-gray-700 hover:border-gray-300 dark:hover:border-gray-600']"
            >
              <div class="flex items-center justify-between">
                <div class="flex items-center">
                  <div v-if="link.icon" class="w-10 h-10 flex items-center justify-center rounded-full bg-gray-100 dark:bg-gray-700 mr-4">
                    <IconFont :type="link.icon" class="w-5 h-5 text-gray-600 dark:text-gray-300" />
                  </div>
                  <div v-else class="w-10 h-10 flex items-center justify-center rounded-full bg-gray-100 dark:bg-gray-700 mr-4">
                    <LinkIcon class="w-5 h-5 text-gray-600 dark:text-gray-300" />
                  </div>
                  <div>
                    <h3 class="font-medium text-gray-900 dark:text-white">{{ link.name }}</h3>
                    <p class="text-sm text-gray-500 dark:text-gray-400 mt-1">{{ link.description || '无描述' }}</p>
                    <a :href="link.url" target="_blank" rel="noopener noreferrer" class="text-blue-600 dark:text-blue-400 hover:underline text-sm mt-1 inline-block">
                      {{ link.url }}
                    </a>
                  </div>
                </div>
                <div class="flex items-center space-x-2">
                  <button 
                    @click="editLink(link)"
                    class="p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
                    title="编辑"
                  >
                    <EditIcon class="w-5 h-5 text-gray-600 dark:text-gray-400" />
                  </button>
                  <button 
                    @click="confirmDeleteLink(link)"
                    class="p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
                    title="删除"
                  >
                    <DeleteIcon class="w-5 h-5 text-gray-600 dark:text-gray-400" />
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- 空状态 -->
          <div v-if="category.links.length === 0" class="text-center py-12">
            <LinkIcon class="w-12 h-12 text-gray-400 dark:text-gray-600 mx-auto mb-4" />
            <p class="text-gray-500 dark:text-gray-400">暂无链接</p>
            <n-button @click="showAddLinkModal = true" type="primary" size="small" class="mt-4">
              添加第一个链接
            </n-button>
          </div>
        </div>
      </div>
    </div>

    <!-- 添加链接弹窗 -->
    <n-modal v-model:show="showAddLinkModal" preset="card" title="添加链接" size="large">
      <div class="space-y-4 py-4">
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">名称</label>
          <n-input v-model:value="newLink.name" placeholder="输入链接名称" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">URL</label>
          <n-input v-model:value="newLink.url" placeholder="输入链接URL" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">图标</label>
          <n-input v-model:value="newLink.icon" placeholder="输入图标名称（可选）" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">描述</label>
          <n-input v-model:value="newLink.description" type="textarea" placeholder="输入链接描述（可选）" :rows="3" />
        </div>
      </div>
      <template #footer>
        <div class="flex justify-end space-x-2">
          <n-button @click="showAddLinkModal = false">取消</n-button>
          <n-button type="primary" @click="addLink" :loading="addingLink">确定</n-button>
        </div>
      </template>
    </n-modal>

    <!-- 编辑链接弹窗 -->
    <n-modal v-model:show="showEditLinkModal" preset="card" title="编辑链接" size="large">
      <div class="space-y-4 py-4">
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">名称</label>
          <n-input v-model:value="editingLink.name" placeholder="输入链接名称" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">URL</label>
          <n-input v-model:value="editingLink.url" placeholder="输入链接URL" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">图标</label>
          <n-input v-model:value="editingLink.icon" placeholder="输入图标名称（可选）" />
        </div>
        <div class="space-y-2">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">描述</label>
          <n-input v-model:value="editingLink.description" type="textarea" placeholder="输入链接描述（可选）" :rows="3" />
        </div>
      </div>
      <template #footer>
        <div class="flex justify-end space-x-2">
          <n-button @click="showEditLinkModal = false">取消</n-button>
          <n-button type="primary" @click="updateLink" :loading="updatingLink">确定</n-button>
        </div>
      </template>
    </n-modal>

    <!-- 删除确认弹窗 -->
    <n-modal v-model:show="showDeleteConfirmModal" preset="card" title="确认删除" size="small">
      <div class="py-4 text-center">
        <p class="text-gray-700 dark:text-gray-300">确定要删除链接 "{{ deletingLink?.name }}" 吗？</p>
      </div>
      <template #footer>
        <div class="flex justify-end space-x-2">
          <n-button @click="showDeleteConfirmModal = false">取消</n-button>
          <n-button type="error" @click="deleteLink" :loading="deleting">确定</n-button>
        </div>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { NInput, NButton, NModal, NSpin } from 'naive-ui';
import IconFont from '../components/IconFont.vue';
import { CategoryService } from '../services/CategoryService';
import type { CategoryModel } from '../models/category';
import type { LinkModel } from '../models/link';

// 路由
const route = useRoute();
const router = useRouter();
const categoryId = ref<string>(route.params.id.toString() || '');

// LinkModel已经定义了完整的链接类型，包含index字段

// 状态
const loading = ref(true);
const error = ref('');
const category = ref<CategoryModel | null>(null);
const isEditing = ref(false);
const editingCategory = reactive<Partial<CategoryModel>>({
  name: '',
  description: '',
  icon: ''
});
const savingInfo = ref(false);

// 拖拽状态
const isDragging = ref<boolean>(false);
const draggedItem = ref<LinkModel | null>(null); // 更新为LinkModel类型
const draggedLink = ref<LinkModel | null>(null);
const dragOverIndex = ref<number | null>(null);

// 链接弹窗
const showAddLinkModal = ref(false);
const showEditLinkModal = ref(false);
const showDeleteConfirmModal = ref(false);

// 链接表单数据
const newLink = reactive<Partial<LinkModel>>({
  name: '',
  url: '',
  icon: '',
  description: ''
});

const editingLink = reactive<Partial<LinkModel>>({
  key: '',
  name: '',
  url: '',
  icon: '',
  description: ''
});

const deletingLink = ref<LinkModel | null>(null);
const addingLink = ref(false);
const updatingLink = ref(false);
const deleting = ref(false);

// 加载分类数据
const loadCategory = async () => {
  try {
    loading.value = true;
    error.value = '';
    const data = await CategoryService.getCategoryById(categoryId.value);
    // 确保数据结构正确并符合CategoryModel类型
    category.value = {
      key: data.key || String(categoryId.value),
      name: data.name || '未命名分类',
      description: data.description,
      icon: data.icon || '',
      index: data.index || 0,
      links: data.links || []
    };
    
    // 初始化编辑数据
    editingCategory.name = category.value.name;
    editingCategory.description = category.value.description;
    editingCategory.icon = category.value.icon;
  } catch (err) {
    error.value = '加载分类失败，请稍后重试';
    console.error('Failed to load category:', err);
    // 提供模拟数据以便页面可以显示
    category.value = {
      key: categoryId.value.toString(),
      name: '分类名称',
      description: '分类描述',
      icon: '',
      index: 0,
      links: []
    };
    editingCategory.name = category.value.name;
    editingCategory.description = category.value.description;
    editingCategory.icon = category.value.icon;
  } finally {
    loading.value = false;
  }
};

// 开始编辑分类信息
const startEdit = () => {
  isEditing.value = true;
};

// 取消编辑
const cancelEdit = () => {
  isEditing.value = false;
  editingCategory.name = category.value?.name || '';
  editingCategory.description = category.value?.description || '';
  editingCategory.icon = category.value?.icon || '';
};

// 保存分类信息
const saveCategoryInfo = async () => {
  if (!category.value) return;
  
  // 验证
  if (!editingCategory.name?.trim()) {
    error.value = '分类名称不能为空';
    return;
  }
  
  try {
    savingInfo.value = true;
    error.value = '';
    
    const categoryData = {
      name: editingCategory.name.trim(),
      description: editingCategory.description?.trim(),
      icon: editingCategory.icon?.trim() || '',
      index: category.value?.index || 0
    } as CategoryModel;
    
    // 调用updateCategory，假设它不返回值
    await CategoryService.updateCategory(categoryData);
    
    // 直接使用更新后的数据更新category
    category.value = {
      ...category.value,
      name: categoryData.name,
      description: categoryData.description,
      icon: categoryData.icon,
      index: categoryData.index
    };
    
    isEditing.value = false;
  } catch (err) {
    error.value = '保存失败，请稍后重试';
    console.error('Failed to update category:', err);
  } finally {
    savingInfo.value = false;
  }
}

// 添加链接
const addLink = async () => {
    // 验证
    if (!newLink.name?.trim() || !newLink.url?.trim()) {
      error.value = '链接名称和URL不能为空';
      return;
    }
    
    // 简单的URL格式验证
    try {
      // 如果URL没有协议，添加https://
      let url = newLink.url.trim();
      if (!url.match(/^https?:\/\//i)) {
        url = 'https://' + url;
      }
      new URL(url);
      // 更新表单中的URL为添加协议后的版本
      newLink.url = url;
    } catch {
      error.value = '请输入有效的链接地址';
      return;
    }
    
    try {
    addingLink.value = true;
    error.value = '';
    
    // 创建符合LinkModel的新链接对象
    const createdLink: LinkModel = {
      key: Date.now().toString(),
      name: newLink.name.trim(),
      url: newLink.url.trim(),
      icon: newLink.icon?.trim(),
      description: newLink.description?.trim(),
      index: category.value?.links.length || 0
    };
    
    // 更新本地数据
    if (category.value) {
      category.value.links.push(createdLink);
    }
    
    // 重置表单
    Object.assign(newLink, { name: '', url: '', icon: '', description: '' });
    showAddLinkModal.value = false;
  } catch (err) {
    error.value = '添加链接失败，请稍后重试';
    console.error('Failed to add link:', err);
  } finally {
    addingLink.value = false;
  }
};

// 编辑链接
const editLink = (link: LinkModel) => {
  Object.assign(editingLink, link);
  showEditLinkModal.value = true;
};

// 更新链接
const updateLink = async () => {
    if (!editingLink.key || !category.value) return;
    
    // 验证
    if (!editingLink.name?.trim() || !editingLink.url?.trim()) {
      error.value = '链接名称和URL不能为空';
      return;
    }
    
    // 简单的URL格式验证
    try {
      // 如果URL没有协议，添加https://
      let url = editingLink.url.trim();
      if (!url.match(/^https?:\/\//i)) {
        url = 'https://' + url;
      }
      new URL(url);
      // 更新表单中的URL为添加协议后的版本
      editingLink.url = url;
    } catch {
      error.value = '请输入有效的链接地址';
      return;
    }
    
    try {
    updatingLink.value = true;
    error.value = '';
    
    // 更新本地数据（由于fetchWithAuth不可用）
    const index = category.value.links.findIndex(l => l.key === editingLink.key);
    if (index !== -1) {
      // 确保更新后的对象符合LinkModel类型
      const updatedLink: LinkModel = {
        ...category.value.links[index],
        name: editingLink.name.trim(),
        url: editingLink.url.trim(),
        icon: editingLink.icon?.trim(),
        description: editingLink.description?.trim()
      };
      category.value.links[index] = updatedLink;
    }
    
    showEditLinkModal.value = false;
  } catch (err) {
    error.value = '更新链接失败，请稍后重试';
    console.error('Failed to update link:', err);
  } finally {
    updatingLink.value = false;
  }
};

// 确认删除链接
const confirmDeleteLink = (link: LinkModel) => {
  deletingLink.value = link;
  showDeleteConfirmModal.value = true;
};

// 删除链接
const deleteLink = async () => {
  if (!deletingLink.value || !category.value) return;
  
  try {
    deleting.value = true;
    error.value = '';
    
    // 更新本地数据（由于fetchWithAuth不可用）
    category.value.links = category.value.links.filter(l => l.key !== deletingLink.value?.key);
    
    showDeleteConfirmModal.value = false;
    deletingLink.value = null;
  } catch (err) {
    error.value = '删除链接失败，请稍后重试';
    console.error('Failed to delete link:', err);
  } finally {
    deleting.value = false;
  }
};

// 拖拽开始
const handleDragStart = (event: DragEvent, link: LinkModel) => {
  isDragging.value = true;
  draggedItem.value = link;
  draggedLink.value = link;
  if (event.dataTransfer) {
    event.dataTransfer.effectAllowed = 'move';
    event.dataTransfer.setData('text/plain', link.key);
  }
};

// 拖拽进入
const handleDragEnter = (event: DragEvent, link: LinkModel) => {
  event.preventDefault();
  if (dragOverIndex.value !== null) {
    dragOverIndex.value = -1; // 重置索引
  }
  if (draggedLink.value?.key !== link.key) {
    // 可以在这里添加视觉反馈
  }
};

// 拖拽离开
const handleDragLeave = (event: DragEvent) => {
  event.preventDefault()
  // 只有当鼠标真正离开元素时才清除拖拽状态
  const target = event.target as Element;
  const relatedTarget = event.relatedTarget as Element;
  if (!target || !relatedTarget || !target.contains(relatedTarget)) {
    dragOverIndex.value = null
  }
};

// 拖拽放下
const handleDrop = async (event: DragEvent, targetLink: LinkModel) => {
  event.preventDefault();
  
  if (!draggedItem.value || draggedItem.value.key === targetLink.key || !category.value) {
    draggedItem.value = null;
    isDragging.value = false;
    return;
  }
  
  try {
    // 重新排序
    const links = [...category.value.links];
    const draggedIndex = links.findIndex(l => l.key === draggedItem.value?.key);
    const targetIndex = links.findIndex(l => l.key === targetLink.key);
    
    if (draggedIndex === -1 || targetIndex === -1) return;
    
    // 移除被拖拽的元素
    const [removed] = links.splice(draggedIndex, 1);
    // 插入到新位置
    links.splice(targetIndex, 0, removed);
    
    // 更新索引
    const updatedLinks = links.map((link, index) => ({ ...link, index }));
    
    // 更新本地状态
    category.value.links = updatedLinks;
    
    // 由于fetchWithAuth不可用，我们只更新本地状态
  } catch (err) {
    error.value = '更新链接顺序失败，请稍后重试';
    console.error('Failed to update link order:', err);
    // 重新加载数据
    await loadCategory();
  } finally {
    isDragging.value = false;
    draggedItem.value = null;
  }
};

// 图标渲染已通过IconFont组件实现

// 返回列表
const goBack = () => {
  router.push('/admin/categories');
};

// 初始化
onMounted(() => {
  loadCategory();
});
</script>

<style scoped>
/* 拖拽相关样式 */
[draggable="true"] {
  cursor: grab;
}

[draggable="true"]:active {
  cursor: grabbing;
}

/* 平滑过渡动画 */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.3s ease;
}

.slide-up-enter-from {
  opacity: 0;
  transform: translateY(20px);
}

.slide-up-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}
</style>