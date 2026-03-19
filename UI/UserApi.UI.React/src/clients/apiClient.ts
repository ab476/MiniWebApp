import axios, { type AxiosInstance, type InternalAxiosRequestConfig } from 'axios';
import type { LoginResponse, Outcome, RefreshTokenRequest } from './types';
import tokenStorage from './tokenStorage';

const apiClient: AxiosInstance = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Variables to handle concurrent 401s
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (token: string) => void;
  reject: (error: any) => void;
}> = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token as string);
    }
  });
  failedQueue = [];
};

// Request Interceptor
apiClient.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = tokenStorage.getAccessToken();
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response Interceptor
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      
      // If a refresh is already happening, queue this request until it finishes
      if (isRefreshing) {
        return new Promise(function (resolve, reject) {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            originalRequest.headers.Authorization = 'Bearer ' + token;
            return apiClient(originalRequest);
          })
          .catch((err) => {
            return Promise.reject(err);
          });
      }

      // Start the refresh process
      originalRequest._retry = true;
      isRefreshing = true;
      const refreshToken = tokenStorage.getRefreshToken();

      if (refreshToken) {
        try {
          const { accessToken } = await refreshAuthToken({ refreshToken });
          
          // Process the queue so waiting requests can proceed
          processQueue(null, accessToken);
          
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
          return apiClient(originalRequest);
        } catch (refreshError) {
          processQueue(refreshError, null);
          
          // CRITICAL: Clear tokens to prevent getting stuck
          tokenStorage.clearTokens(); 
          // window.location.href = '/login'; 
          
          return Promise.reject(refreshError);
        } finally {
          isRefreshing = false;
        }
      } else {
        // No refresh token available, force logout
        tokenStorage.clearTokens();
        return Promise.reject(error);
      }
    }

    return Promise.reject(error);
  }
);

/**
 * Notice we use standard `axios.post` here, NOT `apiClient.post`
 * This prevents the infinite 401 loop!
 */
async function refreshAuthToken(request: RefreshTokenRequest): Promise<LoginResponse> {
  // Use the same baseURL but a clean axios instance to bypass interceptors
  const response = await axios.post<Outcome<LoginResponse>>('/api/auth/refresh-token', request, {
    headers: { 'Content-Type': 'application/json' }
  });

  if (response.data.isSuccess && response.data.value) {
    tokenStorage.setTokens(response.data.value);
    return response.data.value;
  } else {
    throw new Error(response.data.error || 'Token refresh failed');
  }
}

export default apiClient;