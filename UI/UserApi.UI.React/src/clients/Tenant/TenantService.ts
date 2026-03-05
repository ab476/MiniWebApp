import type { Outcome } from "../types";
import apiClient from "../userApiClient";
import type {
    TenantResponse,
    CreateTenantRequest,
    UpdateTenantRequest,
    ActivateTenantRequest,
    DeactivateTenantRequest,
} from "./tenantTypes";

class TenantService {
    private readonly endpoint = "/tenants";

    /** GET /api/tenants/{id} */
    async getById(id: string): Promise<Outcome<TenantResponse>> {
        const response = await apiClient.get<Outcome<TenantResponse>>(
            `${this.endpoint}/${id}`,
        );
        return response.data;
    }

    /** GET /api/tenants */
    async getPaged(
        page = 1,
        pageSize = 20,
    ): Promise<Outcome<TenantResponse[]>> {
        const response = await apiClient.get<Outcome<TenantResponse[]>>(
            this.endpoint,
            {
                params: { page, pageSize },
            },
        );
        return response.data;
    }

    /** POST /api/tenants */
    async create(
        request: CreateTenantRequest,
    ): Promise<Outcome<TenantResponse>> {
        const response = await apiClient.post<Outcome<TenantResponse>>(
            this.endpoint,
            request,
        );
        return response.data;
    }

    /** PUT /api/tenants/{tenantId} */
    async update(
        tenantId: string,
        request: UpdateTenantRequest,
    ): Promise<Outcome> {
        const response = await apiClient.put<Outcome>(
            `${this.endpoint}/${tenantId}`,
            request,
        );
        return response.data;
    }

    /** POST /api/tenants/{tenantId}/activate */
    async activate(
        tenantId: string,
        request: ActivateTenantRequest,
    ): Promise<Outcome> {
        const response = await apiClient.post<Outcome>(
            `${this.endpoint}/${tenantId}/activate`,
            request,
        );
        return response.data;
    }

    /** POST /api/tenants/{tenantId}/deactivate */
    async deactivate(
        tenantId: string,
        request: DeactivateTenantRequest,
    ): Promise<Outcome> {
        const response = await apiClient.post<Outcome>(
            `${this.endpoint}/${tenantId}/deactivate`,
            request,
        );
        return response.data;
    }

    /** DELETE /api/tenants/{tenantId} */
    async delete(tenantId: string): Promise<Outcome> {
        const response = await apiClient.delete<Outcome>(
            `${this.endpoint}/${tenantId}`,
        );
        return response.data;
    }
}

export default new TenantService();

// auth.service.ts
class AuthService {
  async refresh(tokens: { accessToken: string; refreshToken: string }): Promise<Outcome<AuthResponse>> {
    const response = await apiClient.post<Outcome<AuthResponse>>('/auth/refresh', tokens);
    if (response.data.isSuccess && response.data.value) {
      this.setSession(response.data.value);
    }
    return response.data;
  }

  setSession(auth: AuthResponse) {
    localStorage.setItem('accessToken', auth.accessToken);
    localStorage.setItem('refreshToken', auth.refreshToken);
  }

  async logout(): Promise<void> {
    await apiClient.post('/auth/logout');
    localStorage.clear();
    window.location.href = '/login';
  }
}

// apiClient.ts (Interceptor Logic)
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      const refreshToken = localStorage.getItem('refreshToken');
      const accessToken = localStorage.getItem('accessToken');

      const outcome = await authService.refresh({ accessToken, refreshToken });
      if (outcome.isSuccess) {
        originalRequest.headers.Authorization = `Bearer ${outcome.value.accessToken}`;
        return apiClient(originalRequest);
      }
    }
    return Promise.reject(error);
  }
);