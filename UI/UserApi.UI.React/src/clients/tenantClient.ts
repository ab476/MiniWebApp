import apiClient from './apiClient';
import { ApiError } from './ApiError';
import type { 
  Outcome, 
  TenantResponse, 
  CreateTenantRequest, 
  UpdateTenantRequest, 
  ActivateTenantRequest, 
  DeactivateTenantRequest 
} from './types';



class TenantClient {
  /**
   * Retrieves a single tenant by its ID.
   */
  async getById(id: string, signal: AbortSignal): Promise<TenantResponse> {
    const response = await apiClient.get<Outcome<TenantResponse>>(`/tenants/${id}`, { signal });
    if (response.data.isSuccess && response.data.value) {
      return response.data.value;
    } else {
      throw new ApiError(response.data.error || 'Failed to fetch tenant');
    }
  }

  /**
   * Retrieves a paged list of tenants.
   */
  async getPaged(page: number = 1, pageSize: number = 20, signal: AbortSignal): Promise<TenantResponse[]> {
    const response = await apiClient.get<Outcome<TenantResponse[]>>('/tenants', {
      params: { page, pageSize },
      signal
    });
    if (response.data.isSuccess && response.data.value) {
      return response.data.value;
    } else {
      throw new ApiError(response.data.error || 'Failed to fetch tenants');
    }
  }

  /**
   * Creates a new tenant.
   */
  async create(request: CreateTenantRequest, signal: AbortSignal): Promise<TenantResponse> {
    const response = await apiClient.post<Outcome<TenantResponse>>('/tenants', request, { signal });
    if (response.data.isSuccess && response.data.value) {
      return response.data.value;
    } else {
      throw new ApiError(response.data.error || 'Failed to create tenant');
    }
  }

  /**
   * Updates an existing tenant's information.
   */
  async update(tenantId: string, request: UpdateTenantRequest, signal: AbortSignal): Promise<void> {
    const response = await apiClient.put<Outcome<void>>(`/tenants/${tenantId}`, request, { signal });
    if (!response.data.isSuccess) {
      throw new ApiError(response.data.error || 'Failed to update tenant');
    }
  }

  /**
   * Activates a tenant.
   */
  async activate(tenantId: string, request: ActivateTenantRequest, signal?: AbortSignal): Promise<void> {
    const response = await apiClient.post<Outcome<void>>(`/tenants/${tenantId}/activate`, request, { signal });
    if (!response.data.isSuccess) {
      throw new ApiError(response.data.error || 'Failed to activate tenant');
    }
  }

  /**
   * Deactivates a tenant.
   */
  async deactivate(tenantId: string, request: DeactivateTenantRequest, signal?: AbortSignal): Promise<void> {
    const response = await apiClient.post<Outcome<void>>(`/tenants/${tenantId}/deactivate`, request, { signal });
    if (!response.data.isSuccess) {
      throw new ApiError(response.data.error || 'Failed to deactivate tenant');
    }
  }

  /**
   * Deletes a tenant.
   */
  async delete(tenantId: string, signal?: AbortSignal): Promise<void> {
    const response = await apiClient.delete<Outcome<void>>(`/tenants/${tenantId}`, { signal });
    if (!response.data.isSuccess) {
      throw new ApiError(response.data.error || 'Failed to delete tenant');
    }
  }
}

const tenantClient = new TenantClient();
export default tenantClient;