import '@/assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import HighchartsVue from 'highcharts-vue'

import App from '@/App.vue'
import router from './router'


const pinia = createPinia()
const app = createApp(App).use(pinia).use(router).use(HighchartsVue).mount('#app')
