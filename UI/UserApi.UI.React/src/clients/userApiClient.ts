import axios, { type AxiosInstance } from 'axios';

const apiClient: AxiosInstance = axios.create({
  baseURL: 'https://your-api-domain.com/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor for Auth (Optional)
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default apiClient;