import apiClient from './apiClient';
import tokenStorage from './tokenStorage';
import type { LoginRequest, LoginResponse, Outcome } from './types';

class AuthClient {
  async login(request: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post<Outcome<LoginResponse>>('/auth/login', request);
    if (response.data.isSuccess && response.data.value) {
      tokenStorage.setTokens(response.data.value);
      return response.data.value;
    } else {
      throw new Error(response.data.error || 'Login failed');
    }
  }

  

  async logout(): Promise<void> {
    const response = await apiClient.post<Outcome<void>>('/auth/logout');
    if (response.data.isSuccess) {
      tokenStorage.clearTokens();
    } else {
      throw new Error(response.data.error || 'Logout failed');
    }
  }

  getAccessToken(): string | null {
    return tokenStorage.getAccessToken();
  }

  getRefreshToken(): string | null {
    return tokenStorage.getRefreshToken();
  }
}

const authClient = new AuthClient();
export default authClient;
