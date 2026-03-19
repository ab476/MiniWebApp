/**
 * Custom error class for API-related failures.
 */
export class ApiError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'ApiError';
    
    // Required to maintain the correct prototype chain in TypeScript 
    // when extending built-in objects like Error.
    Object.setPrototypeOf(this, ApiError.prototype);
  }
}