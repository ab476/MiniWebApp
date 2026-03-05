// Mirroring your C# Outcome<T> or Outcome
export interface Outcome<T = void> {
  isSuccess: boolean;
  message?: string;
  errors?: string[];
  value?: T; // This will be the TenantResponse or IReadOnlyList
}

