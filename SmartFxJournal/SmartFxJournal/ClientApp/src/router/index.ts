import { createRouter, createWebHistory } from 'vue-router'
import SummaryPageVue from '@/pages/SummaryPage.vue'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
          path: '/',
          name: 'summary',
          component: SummaryPageVue
        },
        {
          path: '/trades',
          name: 'trades',
          // route level code-splitting
          // this generates a separate chunk (About.[hash].js) for this route
          // which is lazy-loaded when the route is visited.
          component: () => import('../pages/TradesPage.vue')
        }
    ] 
})

export default router
