import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
          path: '/',
          name: 'summary',
          component: () => import('@/pages/SummaryPage.vue')
        },
        {
          path: '/trades',
          name: 'trades',
          // route level code-splitting
          // which is lazy-loaded when the route is visited.
          component: () => import('@/pages/TradesPage.vue')
        },
        {
          path: '/reconcile',
          name: 'orderReconcile',
          component: () => import('@/pages/OrderReconcilePage.vue')
        }
    ] 
})

export default router
