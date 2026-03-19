import apiClient from './apiClient';
import { ApiError } from './ApiError';
import type { Outcome, ClaimResponse, GetClaimRequest } from './types';

class ClaimClient {
  async listClaims(): Promise<ClaimResponse[]> {
    const response = await apiClient.get<Outcome<ClaimResponse[]>>('/claims');

    if (response.data.isSuccess && response.data.value) {
      return response.data.value;
    } else {
      throw new ApiError(response.data.error || 'Failed to fetch claims');
    }
  }

  async getClaim(request: GetClaimRequest): Promise<ClaimResponse> {
    const encodedClaimCode = encodeURIComponent(request.claimCode);
    const response = await apiClient.get<Outcome<ClaimResponse>>(
      `/claims/${encodedClaimCode}`
    );

    if (response.data.isSuccess && response.data.value) {
      return response.data.value;
    } else {
      throw new ApiError(
        response.data.error || `Failed to fetch claim: ${request.claimCode}`
      );
    }
  }
}

const claimClient = new ClaimClient();
export default claimClient;
