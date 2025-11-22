<template>
  <div class="py-6 px-4 sm:px-6 lg:px-8">
    <!-- 页面标题 -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6">
      <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">数据导入导出</h1>
      <div class="mt-4 sm:mt-0 flex gap-2">
        <button 
          @click="exportData()" 
          class="inline-flex items-center px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all"
        >
          <Icon icon="ep:download" class="mr-2 h-4 w-4" />
          导出数据
        </button>
      </div>
    </div>

    <!-- 数据统计卡片 -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6 transition-transform hover:-translate-y-1 duration-300">
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400">总链接数</h3>
            <p class="mt-1 text-3xl font-bold text-gray-900 dark:text-white">{{ stats.totalLinks }}</p>
          </div>
          <div class="h-12 w-12 rounded-full bg-blue-100 dark:bg-blue-900 flex items-center justify-center">
            <Icon icon="ep:link" class="h-6 w-6 text-blue-600 dark:text-blue-300" />
          </div>
        </div>
        <div class="mt-4">
          <span 
            class="inline-flex items-center text-sm font-medium"
            :class="stats.linksGrowth >= 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'"
          >
            <Icon icon="ep:trend-up" class="mr-1 h-4 w-4" />
            {{ Math.abs(stats.linksGrowth) }}%
            <span class="ml-1">{{ stats.linksGrowth >= 0 ? '增长' : '下降' }}</span>
          </span>
          <span class="text-xs text-gray-500 dark:text-gray-400 ml-2">相比上月</span>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6 transition-transform hover:-translate-y-1 duration-300">
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400">总分类数</h3>
            <p class="mt-1 text-3xl font-bold text-gray-900 dark:text-white">{{ stats.totalCategories }}</p>
          </div>
          <div class="h-12 w-12 rounded-full bg-purple-100 dark:bg-purple-900 flex items-center justify-center">
            <Icon icon="ep:folder" class="h-6 w-6 text-purple-600 dark:text-purple-300" />
          </div>
        </div>
        <div class="mt-4">
          <span 
            class="inline-flex items-center text-sm font-medium"
            :class="stats.categoriesGrowth >= 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'"
          >
            <Icon icon="ep:trend-up" class="mr-1 h-4 w-4" />
            {{ Math.abs(stats.categoriesGrowth) }}%
            <span class="ml-1">{{ stats.categoriesGrowth >= 0 ? '增长' : '下降' }}</span>
          </span>
          <span class="text-xs text-gray-500 dark:text-gray-400 ml-2">相比上月</span>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6 transition-transform hover:-translate-y-1 duration-300">
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400">总用户数</h3>
            <p class="mt-1 text-3xl font-bold text-gray-900 dark:text-white">{{ stats.totalUsers }}</p>
          </div>
          <div class="h-12 w-12 rounded-full bg-green-100 dark:bg-green-900 flex items-center justify-center">
            <Icon icon="ep:user" class="h-6 w-6 text-green-600 dark:text-green-300" />
          </div>
        </div>
        <div class="mt-4">
          <span 
            class="inline-flex items-center text-sm font-medium"
            :class="stats.usersGrowth >= 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'"
          >
            <Icon icon="ep:trend-up" class="mr-1 h-4 w-4" />
            {{ Math.abs(stats.usersGrowth) }}%
            <span class="ml-1">{{ stats.usersGrowth >= 0 ? '增长' : '下降' }}</span>
          </span>
          <span class="text-xs text-gray-500 dark:text-gray-400 ml-2">相比上月</span>
        </div>
      </div>
    </div>

    <!-- 数据图表 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">链接分类分布</h2>
        <div id="categoryChart" ref="categoryChartRef" class="w-full h-64"></div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">链接增长趋势</h2>
        <div id="growthChart" ref="growthChartRef" class="w-full h-64"></div>
      </div>
    </div>

    <!-- 导入和导出功能 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- 数据导入 -->
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">数据导入</h2>
        <div class="space-y-4">
          <div class="border-2 border-dashed border-gray-300 dark:border-gray-600 rounded-lg p-6 text-center transition-all hover:border-primary/50 cursor-pointer" @click="triggerFileUpload">
            <input 
              ref="fileInputRef" 
              type="file" 
              class="hidden" 
              @change="handleFileUpload" 
              accept=".json"
            />
            <Icon icon="ep:upload" class="h-10 w-10 text-gray-400 mx-auto mb-2" />
            <p class="text-gray-500 dark:text-gray-400">拖放文件到此处，或点击选择文件</p>
            <p class="text-xs text-gray-400 mt-1">支持 JSON 格式</p>
            <div v-if="selectedFile" class="mt-4 flex items-center justify-center">
              <Icon icon="ep:document" class="h-4 w-4 text-primary mr-2" />
              <span class="text-sm text-gray-700 dark:text-gray-300 truncate max-w-xs">{{ selectedFile.name }}</span>
              <span class="ml-2 text-xs text-gray-500 dark:text-gray-400">{{ formatFileSize(selectedFile.size) }}</span>
              <button @click="clearFile" class="ml-2 text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
                <Icon icon="ep:close" class="h-4 w-4" />
              </button>
            </div>
          </div>
          
          <div class="space-y-2">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">导入类型</label>
            <select
              v-model="importType"
              class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
            >
              <option value="links">链接数据</option>
              <option value="categories">分类数据</option>
              <option value="users">用户数据</option>
            </select>
          </div>
          
          <div class="space-y-2">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">导入选项</label>
            <div class="flex items-center">
              <input 
                type="checkbox" 
                id="replaceData" 
                v-model="replaceData" 
                class="h-4 w-4 text-primary focus:ring-primary/50 border-gray-300 rounded"
              >
              <label for="replaceData" class="ml-2 block text-sm text-gray-700 dark:text-gray-300">替换现有数据</label>
            </div>
            <p class="text-xs text-gray-500 dark:text-gray-400">注意：选择此选项将覆盖现有数据，请谨慎操作</p>
          </div>
          
          <button 
            @click="importData" 
            :disabled="!selectedFile || importing"
            class="w-full inline-flex items-center justify-center px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all disabled:opacity-70 disabled:cursor-not-allowed"
          >
            <n-spin v-if="importing" size="small" class="mr-2" />
            {{ importing ? '导入中...' : '开始导入' }}
          </button>
        </div>
      </div>

      <!-- 数据导出 -->
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">数据导出</h2>
        <div class="space-y-4">
          <div class="space-y-2">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">导出内容</label>
            <div class="space-y-2">
              <div class="flex items-center">
                <input 
                  type="checkbox" 
                  id="exportLinks" 
                  v-model="exportOptions.links" 
                  class="h-4 w-4 text-primary focus:ring-primary/50 border-gray-300 rounded"
                >
                <label for="exportLinks" class="ml-2 block text-sm text-gray-700 dark:text-gray-300">链接数据</label>
              </div>
              <div class="flex items-center">
                <input 
                  type="checkbox" 
                  id="exportCategories" 
                  v-model="exportOptions.categories" 
                  class="h-4 w-4 text-primary focus:ring-primary/50 border-gray-300 rounded"
                >
                <label for="exportCategories" class="ml-2 block text-sm text-gray-700 dark:text-gray-300">分类数据</label>
              </div>
              <div class="flex items-center">
                <input 
                  type="checkbox" 
                  id="exportUsers" 
                  v-model="exportOptions.users" 
                  class="h-4 w-4 text-primary focus:ring-primary/50 border-gray-300 rounded"
                >
                <label for="exportUsers" class="ml-2 block text-sm text-gray-700 dark:text-gray-300">用户数据</label>
              </div>
            </div>
          </div>
          
          <div class="space-y-2">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">导出格式</label>
            <select
              v-model="exportFormat"
              class="block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 py-2.5 px-4 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all"
            >
              <option value="csv">CSV</option>
              <option value="xlsx">Excel</option>
              <option value="json">JSON</option>
            </select>
          </div>
          
          <div class="space-y-2">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-200">导出选项</label>
            <div class="flex items-center">
              <input 
                type="checkbox" 
                id="includeMetadata" 
                v-model="includeMetadata" 
                class="h-4 w-4 text-primary focus:ring-primary/50 border-gray-300 rounded"
              >
              <label for="includeMetadata" class="ml-2 block text-sm text-gray-700 dark:text-gray-300">包含元数据</label>
            </div>
          </div>
          
          <button 
            @click="exportData" 
            :disabled="!hasSelectedExportItems || exporting"
            class="w-full inline-flex items-center justify-center px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all disabled:opacity-70 disabled:cursor-not-allowed"
          >
            <n-spin v-if="exporting" size="small" class="mr-2" />
            {{ exporting ? '导出中...' : '开始导出' }}
          </button>
        </div>
      </div>
    </div>

    <!-- 最近导入导出记录 -->
    <div class="mt-8">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">最近导入导出记录</h2>
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  操作类型
                </th>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  数据类型
                </th>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  操作人
                </th>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  操作时间
                </th>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  状态
                </th>
                <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  详情
                </th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="record in recentRecords" :key="record.id" class="hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full" 
                    :class="record.type === 'import' 
                      ? 'bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200' 
                      : 'bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200'"
                  >
                    {{ record.type === 'import' ? '导入' : '导出' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-700 dark:text-gray-300">
                  {{ getDataTypeText(record.dataType) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-700 dark:text-gray-300">
                  {{ record.operator }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                  {{ formatDate(record.timestamp) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full" 
                    :class="record.status === 'success' 
                      ? 'bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200' 
                      : 'bg-red-100 dark:bg-red-900 text-red-800 dark:text-red-200'"
                  >
                    {{ record.status === 'success' ? '成功' : '失败' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-700 dark:text-gray-300">
                  {{ record.details }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        
        <div v-if="recentRecords.length === 0" class="p-8 text-center">
          <Icon icon="ep:time" class="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <p class="text-gray-500 dark:text-gray-400">暂无导入导出记录</p>
        </div>
      </div>
    </div>

    <!-- 导入成功提示 -->
    <n-modal
      v-model:show="showImportSuccessModal"
      title="导入成功"
      preset="dialog"
      size="medium"
    >
      <div class="space-y-4">
        <div class="flex items-center justify-center">
          <div class="h-16 w-16 rounded-full bg-green-100 dark:bg-green-900 flex items-center justify-center">
            <Icon icon="ep:success" class="h-8 w-8 text-green-600 dark:text-green-300" />
          </div>
        </div>
        <div class="text-center">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">数据导入成功</h3>
          <p class="mt-2 text-gray-500 dark:text-gray-400">{{ importSuccessMessage }}</p>
        </div>
        <div class="space-y-2">
          <div v-if="importResult.total > 0" class="text-sm text-gray-700 dark:text-gray-300">
            <span class="font-medium">导入记录数:</span> {{ importResult.total }}
          </div>
          <div v-if="importResult.success > 0" class="text-sm text-green-600 dark:text-green-400">
            <span class="font-medium">成功记录:</span> {{ importResult.success }}
          </div>
          <div v-if="importResult.failed > 0" class="text-sm text-red-600 dark:text-red-400">
            <span class="font-medium">失败记录:</span> {{ importResult.failed }}
          </div>
        </div>
      </div>
      <template #footer>
        <button 
          @click="showImportSuccessModal = false" 
          class="px-4 py-2 border border-transparent rounded-lg shadow-sm text-sm font-medium text-white bg-primary hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary/50 transition-all"
        >
          确定
        </button>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted, nextTick, watch } from 'vue'
import { useMessage } from 'naive-ui'
import { NSpin, NModal } from 'naive-ui'
import { Icon } from '@iconify/vue'
import * as echarts from 'echarts'
import { DataService } from '../services/DataService'

interface ImportResult {
  total: number
  success: number
  failed: number
}

// 消息提示
const message = useMessage()

// 状态
const loading = ref(false)
const importing = ref(false)
const exporting = ref(false)
const selectedFile = ref<File | null>(null)
const showImportSuccessModal = ref(false)
const isValidJsonFile = ref(false)

// 导入配置
const importType = ref('links')
const replaceData = ref(false)
const importSuccessMessage = ref('')
const importResult = ref<ImportResult>({
  total: 0,
  success: 0,
  failed: 0
})

// 导出配置
const exportOptions = reactive({
  links: true,
  categories: true,
  users: false
})
const exportFormat = ref('csv')
const includeMetadata = ref(true)

// 数据统计
const stats = reactive({
  totalLinks: 0,
  totalCategories: 0,
  totalUsers: 0,
  linksGrowth: 0,
  categoriesGrowth: 0,
  usersGrowth: 0
})

// 最近导入导出记录
const recentRecords = ref([
  // 模拟数据
  {
    id: '1',
    type: 'import',
    dataType: 'links',
    operator: 'admin',
    timestamp: new Date().toISOString(),
    status: 'success',
    details: '导入了15条链接数据'
  },
  {
    id: '2',
    type: 'export',
    dataType: 'categories,links',
    operator: 'admin',
    timestamp: new Date(Date.now() - 86400000).toISOString(), // 昨天
    status: 'success',
    details: '导出了所有链接和分类数据'
  }
])

// 图表引用
const categoryChartRef = ref<HTMLElement | null>(null)
const growthChartRef = ref<HTMLElement | null>(null)
let categoryChart: echarts.ECharts | null = null
let growthChart: echarts.ECharts | null = null

// DOM引用
const fileInputRef = ref<HTMLInputElement | null>(null)

// 计算属性
const hasSelectedExportItems = computed(() => {
  return exportOptions.links || exportOptions.categories || exportOptions.users
})

// 方法
const loadStats = async () => {
  try {
    loading.value = true
    // 实际项目中从API获取数据统计
    // const data = await DataService.getStats()
    // stats.value = data
    
    // 模拟数据
    stats.totalLinks = 156
    stats.totalCategories = 12
    stats.totalUsers = 8
    stats.linksGrowth = 15
    stats.categoriesGrowth = 5
    stats.usersGrowth = 2
  } catch (error) {
    message.error('加载统计数据失败')
  } finally {
    loading.value = false
  }
}

const initCharts = async () => {
  await nextTick()
  
  // 初始化分类分布图表
  if (categoryChartRef.value) {
    categoryChart = echarts.init(categoryChartRef.value)
    const isDarkMode = document.documentElement.classList.contains('dark')
    
    const option = {
      tooltip: {
        trigger: 'item',
        formatter: '{a} <br/>{b}: {c} ({d}%)'
      },
      legend: {
        orient: 'vertical',
        left: 'left',
        textStyle: {
          color: isDarkMode ? '#e0e0e0' : '#333'
        }
      },
      series: [
        {
          name: '链接分类',
          type: 'pie',
          radius: '50%',
          data: [
            { value: 45, name: '学习资料' },
            { value: 30, name: '技术文档' },
            { value: 25, name: '新闻资讯' },
            { value: 20, name: '娱乐休闲' },
            { value: 15, name: '其他' },
            { value: 21, name: '工作相关' }
          ],
          emphasis: {
            itemStyle: {
              shadowBlur: 10,
              shadowOffsetX: 0,
              shadowColor: 'rgba(0, 0, 0, 0.5)'
            }
          },
          itemStyle: {
            borderRadius: 5,
            borderColor: isDarkMode ? '#2d3748' : '#fff',
            borderWidth: 2
          },
          label: {
            color: isDarkMode ? '#e0e0e0' : '#333'
          }
        }
      ]
    }
    
    categoryChart.setOption(option)
  }
  
  // 初始化增长趋势图表
  if (growthChartRef.value) {
    growthChart = echarts.init(growthChartRef.value)
    const isDarkMode = document.documentElement.classList.contains('dark')
    
    const option = {
      tooltip: {
        trigger: 'axis'
      },
      grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        containLabel: true
      },
      xAxis: {
        type: 'category',
        boundaryGap: false,
        data: ['1月', '2月', '3月', '4月', '5月', '6月'],
        axisLabel: {
          color: isDarkMode ? '#e0e0e0' : '#333'
        }
      },
      yAxis: {
        type: 'value',
        axisLabel: {
          color: isDarkMode ? '#e0e0e0' : '#333'
        }
      },
      series: [
        {
          name: '链接数量',
          type: 'line',
          smooth: true,
          data: [120, 132, 101, 134, 90, 156],
          areaStyle: {
            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
              {
                offset: 0,
                color: 'rgba(59, 130, 246, 0.4)'
              },
              {
                offset: 1,
                color: 'rgba(59, 130, 246, 0.1)'
              }
            ])
          },
          lineStyle: {
            color: 'rgba(59, 130, 246, 1)',
            width: 3
          },
          itemStyle: {
            color: 'rgba(59, 130, 246, 1)'
          }
        }
      ]
    }
    
    growthChart.setOption(option)
  }
}

const resizeCharts = () => {
  categoryChart?.resize()
  growthChart?.resize()
}

const triggerFileUpload = () => {
  fileInputRef.value?.click()
}

const handleFileUpload = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    selectedFile.value = target.files[0]!
    // 验证文件格式
    const file = target.files[0]!
    const isJsonFile = file.type === 'application/json' || file.name.endsWith('.json')
    const isFileSizeValid = file.size <= 10 * 1024 * 1024
    isValidJsonFile.value = isJsonFile && isFileSizeValid
  }
}

const clearFile = () => {
  selectedFile.value = null
  if (fileInputRef.value) {
    fileInputRef.value.value = ''
  }
}

const importData = async () => {
  if (!selectedFile.value) {
    message.warning('请选择要导入的文件')
    return
  }
  
  try {
    importing.value = true
    
    // 验证文件格式
    if (!selectedFile.value) {
      message.error('请选择要导入的文件')
      return
    }
    
    const file = selectedFile.value
    if (!isValidJsonFile.value) {
      if (file.type !== 'application/json' && !file.name.endsWith('.json')) {
        message.error('请选择有效的JSON文件')
      } else if (file.size > 10 * 1024 * 1024) {
        message.error('文件大小不能超过10MB')
      }
      return
    }
    
    // 验证文件内容
    const isValidJson = await DataService.readAndValidateJsonFile(selectedFile.value)
    if (!isValidJson) {
      message.error('文件内容不是有效的JSON格式')
      return
    }
    
    // 调用API导入数据
    const result = await DataService.uploadData(selectedFile.value)
    
    if (result.success) {
      importResult.value = {
        total: 1,
        success: 1,
        failed: 0
      }
      importSuccessMessage.value = result.message || '数据导入成功'
      showImportSuccessModal.value = true
      
      // 清除选择的文件
      clearFile()
      
      // 重新加载统计数据
      await loadStats()
      
      // 添加导入记录
      addImportExportRecord('import', 'all', 'success', result.message)
    }
    
  } catch (error: any) {
    message.error(error.message || '导入失败，请重试')
    // 添加失败记录
    addImportExportRecord('import', 'all', 'failed', error.message || '导入失败')
  } finally {
    importing.value = false
  }
}

const exportData = async () => {
  try {
    exporting.value = true
    
    // 调用API导出数据
    await DataService.downloadData()
    
    // 获取选中的数据类型
    const selectedTypes = Object.entries(exportOptions)
      .filter(([_, value]) => value)
      .map(([key]) => key)
    
    const dataTypeText = selectedTypes.length > 0 
      ? selectedTypes.join(',') 
      : 'all'
    
    message.success('导出成功')
    
    // 添加导出记录
    addImportExportRecord('export', dataTypeText, 'success', `导出了${selectedTypes.length > 0 ? selectedTypes.join('、') : '全部'}数据`)
    
  } catch (error: any) {
    message.error(error.message || '导出失败，请重试')
    // 添加失败记录
    addImportExportRecord('export', 'all', 'failed', error.message || '导出失败')
  } finally {
    exporting.value = false
  }
}

const getDataTypeText = (dataType: string): string => {
  const types = {
    'links': '链接数据',
    'categories': '分类数据',
    'users': '用户数据',
    'categories,links': '分类和链接数据'
  }
  return types[dataType as keyof typeof types] || dataType
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

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes'
  
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

const addImportExportRecord = (type: 'import' | 'export', dataType: string, status: 'success' | 'failed', details: string) => {
  const newRecord = {
    id: Date.now().toString(),
    type,
    dataType,
    operator: '当前用户', // 实际项目中应该从登录信息获取
    timestamp: new Date().toISOString(),
    status,
    details
  }
  
  // 添加到记录列表开头
  recentRecords.value.unshift(newRecord)
  
  // 只保留最近10条记录
  if (recentRecords.value.length > 10) {
    recentRecords.value = recentRecords.value.slice(0, 10)
  }
}

// 暗黑模式变化监听
const handleDarkModeChange = () => {
  // 重新初始化图表以适应暗黑模式
  initCharts()
}

// 生命周期
onMounted(async () => {
  await loadStats()
  await initCharts()
  window.addEventListener('resize', resizeCharts)
  
  // 监听暗黑模式变化
  const observer = new MutationObserver((mutations) => {
    mutations.forEach(mutation => {
      if (mutation.attributeName === 'class') {
        handleDarkModeChange()
      }
    })
  })
  
  observer.observe(document.documentElement, { attributes: true })
})

onUnmounted(() => {
  window.removeEventListener('resize', resizeCharts)
  categoryChart?.dispose()
  growthChart?.dispose()
})

// 监听导入结果变化
watch(importResult, (newResult) => {
  if (newResult.failed > 0) {
    importSuccessMessage.value += `，但有 ${newResult.failed} 条记录导入失败`
  }
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

/* 自定义文件上传区域样式 */
.border-dashed {
  border-style: dashed;
}

.border-dashed:hover {
  background-color: rgba(59, 130, 246, 0.04);
}

/* 统计卡片悬停效果 */
.transition-transform {
  transition-property: transform;
}

.hover\:-translate-y-1:hover {
  transform: translateY(-4px);
}

/* 自定义滚动条 */
.overflow-x-auto::-webkit-scrollbar {
  height: 6px;
}

.overflow-x-auto::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

.overflow-x-auto::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 10px;
}

.overflow-x-auto::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

.dark .overflow-x-auto::-webkit-scrollbar-track {
  background: #2d3748;
}

.dark .overflow-x-auto::-webkit-scrollbar-thumb {
  background: #4a5568;
}

.dark .overflow-x-auto::-webkit-scrollbar-thumb:hover {
  background: #718096;
}
</style>