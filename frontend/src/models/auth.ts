// 认证相关的数据模型定义
export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
}

export interface TokenResponse {
  accessToken: string;
  tokenType: string;
}

export interface OAuthCallbackRequest {
  code: string;
  state: string;
}

export interface TokenValidationResponse {
  isValid: boolean;
  username?: string;
  role?: string;
  userId?: string;
}