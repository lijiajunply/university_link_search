import { defineStore } from 'pinia'

export const useSearchEngineStore = defineStore('searchEngine', {
  state: () => ({
    currentEngineKey: '1' as string
  }),
  
  actions: {
    initCurrentEngine() {
      const savedEngineNum = localStorage.getItem('engine_num')
      if (savedEngineNum !== null) {
        this.currentEngineKey = (parseInt(savedEngineNum) + 1).toString()
      }
    },
    
    setCurrentEngine(key: string) {
      this.currentEngineKey = key
        localStorage.setItem('engine_num', (parseInt(key) - 1).toString())
    }
  }
})