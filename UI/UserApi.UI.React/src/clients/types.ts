// Mirroring your C# Outcome<T> or Outcome
export interface Outcome<T = void> {
  isSuccess: boolean;
  message?: string;
  error?: string;
  value?: T; // This will be the TenantResponse or IReadOnlyList
}

// Based on the C# code
export interface LoginRequest {
  identifier: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

/**
 * Generic wrapper for API responses.
 */
export interface Outcome<T = void> {
  value?: T;
  isSuccess: boolean;
  error?: string;
  statusCode?: number;
}

/**
 * Represents the public-facing tenant information.
 */
export interface TenantResponse {
  id: string; // Guid maps to string in TS
  name: string;
  domain: string | null;
  isActive: boolean;
  createdAt: string; // ISO Date strings
  updatedAt: string | null;
}

/**
 * Data required to create a new tenant.
 */
export interface CreateTenantRequest {
  name: string;
  domain?: string | null;
}

/**
 * Data required to update an existing tenant.
 */
export interface UpdateTenantRequest {
  name: string;
  domain?: string | null;
}

/**
 * Request to activate a tenant.
 */
export interface ActivateTenantRequest {
  tenantId: string;
}

/**
 * Request to deactivate a tenant.
 */
export interface DeactivateTenantRequest {
  tenantId: string;
}


export interface ClaimResponse {
  claimCode: string;
  description: string | null;
  category: string;
}

export interface GetClaimRequest {
  claimCode: string;
}