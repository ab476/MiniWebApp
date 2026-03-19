import type { Outcome } from "../../types";
import apiClient from "../../apiClient";
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