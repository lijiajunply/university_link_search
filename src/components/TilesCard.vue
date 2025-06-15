<script setup>
import {onMounted, ref} from 'vue'
import AppleCard from "./AppleCard.vue";

const todos = ref([])
const clipboards = ref([])
let token = localStorage.getItem('token')
let isShowTiles = localStorage.getItem('is-show-tiles') === 'true'
console.log(isShowTiles)
let isShow = isShowTiles && (token === null || token.length !== 0)
console.log(isShow)
const isLoading = ref(isShow)
const newTodo = ref({
  title: '',
  status: false,
  description: '',
  startTime: '',
  endTime: '',
})
const newClipboard = ref('')

const addClipboard = async () => {
  // 先显现一个对话框 Vue.js 网页

  clipboards.value.push(newClipboard.value)
  newClipboard.value = ''
}

const addTodo = () => {
  if (newTodo.value.title.trim() === '') {
    return
  }

  todos.value.push({
    title: newTodo.value.title,
    status: false,
    description: '',
    startTime: '',
    endTime: '',
  })

  newTodo.value.title = ''
}

const removeTodo = (index) => {
  todos.value.splice(index, 1)
}

onMounted(() => {
  if (isLoading.value) {
    let userJson = localStorage.getItem('user')
    if (userJson) {
      let user = JSON.parse(userJson)
      console.log(user)
      todos.value = user.Todos
      clipboards.value = user.Clipboard
    }
  }
})
</script>

<template>
  <div v-if="isLoading">
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <!-- Todo 列表 -->
      <AppleCard>
        <template #title>
          <div class="font-bold text-[30px] leading-[1.35em]">
            Todo
          </div>
        </template>
        <template #context>
          <div class="p-5">
            <ul class="space-y-2">
              <li
                  v-for="(todo, index) in todos"
                  :key="index"
                  class="flex items-center p-3 rounded-lg hover:bg-gray-50 transition-colors"
                  :class="{'bg-blue-50': todo.Status}"
              >
                <input
                    type="checkbox"
                    v-model="todo.Status"
                    class="h-5 w-5 text-blue-500 rounded focus:ring-blue-400 mr-3"
                >
                <span
                    class="flex-1"
                    :class="{'line-through text-gray-400': todo.Status}"
                >
                  {{ todo.Title }}
                </span>
                <button
                    @click="removeTodo(index)"
                    class="text-gray-400 hover:text-red-500 transition-colors"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                       stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                  </svg>
                </button>
              </li>
            </ul>

            <div v-if="todos.length === 0" class="text-center py-4 text-gray-400">
              暂无待办事项
            </div>
          </div>
        </template>
      </AppleCard>

      <AppleCard>
        <template #title>
          <div class="font-bold text-[30px] leading-[1.35em]">
            剪贴板
          </div>
        </template>
        <template #context>
          <div class="p-5">
            <ul class="space-y-2">
              <li
                  v-for="(clipboard, index) in clipboards"
                  :key="index"
              >
                <div class="flex items-center p-3 rounded-lg hover:bg-gray-50 transition-colors">
                  <span class="flex-1">{{ clipboard }}</span>
                  <button
                      class="px-4 py-2 bg-blue-500 text-white rounded-r-lg hover:bg-blue-600 transition-colors"
                  >
                    复制
                  </button>
                </div>
              </li>
            </ul>

            <div class="text-center py-4 text-gray-400">
              <div v-if="clipboards.length === 0">
                暂无剪贴内容
              </div>

              <div class="mt-2">
                <div class="flex mb-4">
                  <input
                      v-model="newClipboard"
                      @keyup.enter="addClipboard"
                      placeholder="添加剪贴板..."
                      class="flex-1 px-4 py-2 rounded-l-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500"
                  >
                  <button
                      @click="addClipboard"
                      class="px-4 py-2 bg-blue-500 text-white rounded-r-lg hover:bg-blue-600 transition-colors"
                  >
                    添加
                  </button>
                </div>
              </div>
            </div>
          </div>
        </template>
      </AppleCard>
    </div>
  </div>
</template>

<style scoped>

</style>