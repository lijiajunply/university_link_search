import { defineStore } from 'pinia'

export const useSettingStore = defineStore('setting', {
  state: () => ({
    // 搜索引擎相关状态
    currentEngineKey: '1' as string,
    // TilesCard 相关状态
    showTiles: localStorage.getItem('is-show-tiles') === 'true',
    token: localStorage.getItem('token') || '',
    user: null as any,
    todos: [] as any[],
    clipboards: [] as string[],
  }),
  
  getters: {
    isLoading: (state) => {
      return state.showTiles && (state.token === null || state.token.length !== 0)
    }
  },
  
  actions: {
    // 搜索引擎相关操作
    initCurrentEngine() {
      const savedEngineNum = localStorage.getItem('engine_num')
      if (savedEngineNum !== null) {
        this.currentEngineKey = (parseInt(savedEngineNum) + 1).toString()
      }
    },
    
    setCurrentEngine(key: string) {
      this.currentEngineKey = key
      localStorage.setItem('engine_num', (parseInt(key) - 1).toString())
    },
    
    // TilesCard 相关操作
    setShowTiles(show: boolean) {
      this.showTiles = show
      localStorage.setItem('is-show-tiles', show.toString())
    },
    
    setToken(token: string) {
      this.token = token
      localStorage.setItem('token', token)
    },
    
    loadUser() {
      if (this.isLoading) {
        const userJson = localStorage.getItem('user')
        if (userJson) {
          this.user = JSON.parse(userJson)
          this.todos = this.user.Todos || []
          this.clipboards = this.user.Clipboard || []
        }
      }
    },
    
    saveUser() {
      if (this.user) {
        this.user.Todos = this.todos
        this.user.Clipboard = this.clipboards
        localStorage.setItem('user', JSON.stringify(this.user))
      }
    },
    
    addClipboard(text: string) {
      this.clipboards.push(text)
      this.saveUser()
    },
    
    removeTodo(index: number) {
      this.todos.splice(index, 1)
      this.saveUser()
    }
  }
})
