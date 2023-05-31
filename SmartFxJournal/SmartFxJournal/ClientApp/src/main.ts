import '@/assets/main.css'

import { createApp } from 'vue'
import App from '@/App.vue'

const app = createApp(App)
app.mount('#app')
app.config.globalProperties.SelectedAccountNumber = 0
