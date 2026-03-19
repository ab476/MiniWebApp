import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig(({ mode }) => {
  // Load env file based on `mode` in the current working directory.
  // Set the third parameter to '' to load all envs regardless of the `VITE_` prefix.
  const env = loadEnv(mode, process.cwd());

  return {
    plugins: [react()],
    server: {
      proxy: {
        // Proxying requests starting with /api to your backend
        '/api': {
          target: env.VITE_API_URL || 'http://localhost:5000',
          changeOrigin: true,
          secure: false,
          // If your backend API does not have the "/api" prefix in its routes, 
          // uncomment the line below to strip it:
          // rewrite: (path) => path.replace(/^\/api/, ''),
        },
      },
    },
    resolve: {
    alias: {
      // These must match your tsconfig.json paths
      "@clients": path.resolve(__dirname, "./src/clients"),
      "@features": path.resolve(__dirname, "./src/features"),
      "@components": path.resolve(__dirname, "./src/components"),
    },
  },
  };
});
