export interface PermissionResponse {
  id: string;
  name: string;
  description: string;
  createdAt: string;
}

export interface PagedRequest {
  pageNumber: number;
  pageSize: number;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
}

export interface CreatePermissionRequest {
  name: string;
  description: string;
}

export type UpdatePermissionRequest = CreatePermissionRequest;