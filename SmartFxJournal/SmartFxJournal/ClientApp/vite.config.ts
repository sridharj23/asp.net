import { fileURLToPath, URL } from 'node:url'
import mkcert from 'vite-plugin-mkcert'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [vue(), mkcert()],
    build: {
        outDir: "../wwwroot/client",
        emptyOutDir: true,
    },
    server: {
        port: 7050,
        https: true,
        proxy: {
            '/api/': {
                target: 'https://localhost:5000',
                secure: false
            }
        }
    },
    resolve: {
        alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    }
})
