import {createApp} from 'vue'

import './style.css'
import 'vfonts/Lato.css'
import 'vfonts/FiraCode.css'
import {createPinia} from 'pinia'
import VueLazyload from 'vue-lazyload'
import router from './router.ts'

import App from './App.vue'

const pinia = createPinia()
const app = createApp(App)

app.use(pinia)
app.use(VueLazyload)
app.use(router)
app.mount('#app')