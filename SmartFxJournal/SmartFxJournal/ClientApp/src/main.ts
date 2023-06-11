import '@/assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import HighchartsVue from 'highcharts-vue'
import HighCharts from 'highcharts'
import stockInit from 'highcharts/modules/stock'

import App from '@/App.vue'
import router from './router'

stockInit(HighCharts)

const pinia = createPinia()
const app = createApp(App).use(pinia).use(router).use(HighchartsVue, {highcharts: HighCharts}).mount('#app')
