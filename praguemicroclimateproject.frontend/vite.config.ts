import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [plugin()],
  server: {
    port: 49586,
    proxy: {
      '/api': {
        target: 'https://localhost:7000',
        changeOrigin: true,
        secure: false,
      },
    },
  },
});