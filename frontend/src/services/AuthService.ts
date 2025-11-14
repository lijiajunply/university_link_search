import { url } from "./UrlService";
import type { LoginRequest, RegisterRequest, TokenResponse, TokenValidationResponse, OAuthCallbackRequest } from "../models/auth";


// 通用请求方法，处理认证头
async function fetchWithAuth(endpoint: string, options: RequestInit = {}) {
  const token = localStorage.getItem('authToken');
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...options.headers as Record<string, string>,
  };

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  return fetch(`${url}${endpoint}`, {
    ...options,
    headers,
  });
}

export class AuthService {
  // 用户名密码登录
  public static async login(loginRequest: LoginRequest): Promise<TokenResponse> {
    const response = await fetch(`${url}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(loginRequest)
    });

    if (!response.ok) {
      try {
        const error = await response.json();
        throw new Error(error.message || '登录失败');
      } catch (jsonError) {
        throw new Error('登录失败');
      }
    }

    const data = await response.json();
    // 保存token到本地存储
    localStorage.setItem('authToken', data.accessToken);
    return data;
  }

  // 用户注册
  public static async register(registerRequest: RegisterRequest): Promise<{ message: string }> {
    const response = await fetch(`${url}/auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(registerRequest)
    });

    if (!response.ok) {
      try {
        const error = await response.json();
        throw new Error(error.message || '注册失败');
      } catch (jsonError) {
        throw new Error('注册失败');
      }
    }

    return await response.json();
  }

  // 刷新令牌
  public static async refreshToken(): Promise<TokenResponse> {
    const response = await fetchWithAuth('/auth/refresh-token', {
      method: 'POST'
    });

    if (!response.ok) {
      try {
        const error = await response.json();
        throw new Error(error.message || '刷新令牌失败');
      } catch (jsonError) {
        // 刷新失败时清除本地token
        localStorage.removeItem('authToken');
        throw new Error('刷新令牌失败');
      }
    }

    const data = await response.json();
    // 更新token
    localStorage.setItem('authToken', data.accessToken);
    return data;
  }

  // 验证令牌
  public static async validateToken(): Promise<TokenValidationResponse> {
    const response = await fetchWithAuth('/auth/validate-token');

    if (!response.ok) {
      try {
        const error = await response.json();
        // 令牌无效时清除本地token
        localStorage.removeItem('authToken');
        throw new Error(error.message || '验证令牌失败');
      } catch (jsonError) {
        localStorage.removeItem('authToken');
        throw new Error('验证令牌失败');
      }
    }

    return await response.json();
  }

  // 登出
  public static async logout(): Promise<{ message: string }> {
    try {
      const response = await fetchWithAuth('/auth/logout', {
        method: 'POST'
      });

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || '登出失败');
      }

      return await response.json();
    } finally {
      // 无论如何都清除本地存储的token
      localStorage.removeItem('authToken');
    }
  }

  // 获取OAuth授权URL
  public static getAuthorizeUrl(redirectUri?: string): string {
    const baseUrl = `${url}/auth/authorize`;
    if (redirectUri) {
      return `${baseUrl}?redirectUri=${encodeURIComponent(redirectUri)}`;
    }
    return baseUrl;
  }

  // 处理OAuth回调
  public static async handleOAuthCallback(code: string, state: string): Promise<TokenResponse> {
    if (!code) {
      throw new Error('授权码不能为空');
    }
    
    const response = await fetch(`${url}/auth/callback?code=${encodeURIComponent(code)}&state=${encodeURIComponent(state)}`);

    if (!response.ok) {
      try {
        const error = await response.json();
        throw new Error(error.message || 'OAuth回调处理失败');
      } catch (jsonError) {
        throw new Error('OAuth回调处理失败');
      }
    }

    const data = await response.json();
    // 保存token到本地存储
    localStorage.setItem('authToken', data.accessToken);
    return data;
  }

  // 检查是否已登录
  public static isLoggedIn(): boolean {
    return !!localStorage.getItem('authToken');
  }

  // 获取当前登录用户信息
  public static async getCurrentUserInfo(): Promise<TokenValidationResponse | null> {
    if (!this.isLoggedIn()) {
      return null;
    }

    try {
      return await this.validateToken();
    } catch (error) {
      // 验证失败，清除token
      localStorage.removeItem('authToken');
      return null;
    }
  }
}