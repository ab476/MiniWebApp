export interface TenantResponse {
  id: string;
  name: string;
  isActive: boolean;
  // Add other properties from your C# TenantResponse model
}

export interface CreateTenantRequest {
  name: string;
  // Add other properties
}

export interface UpdateTenantRequest {
  name: string;
}

export interface ActivateTenantRequest {
  tenantId: string;
  reason?: string;
}

export interface DeactivateTenantRequest {
  tenantId: string;
  reason?: string;
}