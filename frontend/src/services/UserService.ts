import { url } from "./UrlService";
import type { UserModel } from "../models/user";

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

export class UserService {
  // 获取所有用户（需要管理员权限）
  public static async getAllUsers(): Promise<UserModel[]> {
    const response = await fetchWithAuth('/user');

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '获取用户列表失败');
    }

    return await response.json();
  }

  // 根据ID获取用户
  public static async getUserById(id: string): Promise<UserModel> {
    const response = await fetchWithAuth(`/user/${id}`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '获取用户失败');
    }

    return await response.json();
  }

  // 根据用户名获取用户
  public static async getUserByUsername(username: string): Promise<UserModel> {
    const response = await fetchWithAuth(`/user/username/${encodeURIComponent(username)}`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '获取用户失败');
    }

    return await response.json();
  }

  // 创建用户（需要管理员权限）
  public static async createUser(user: UserModel): Promise<UserModel> {
    const response = await fetchWithAuth('/user', {
      method: 'POST',
      body: JSON.stringify(user)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '创建用户失败');
    }

    return await response.json();
  }

  // 更新用户
  public static async updateUser(id: string, user: UserModel): Promise<void> {
    const response = await fetchWithAuth(`/user/${id}`, {
      method: 'PUT',
      body: JSON.stringify(user)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '更新用户失败');
    }
  }

  // 删除用户（需要管理员权限）
  public static async deleteUser(id: string): Promise<void> {
    const response = await fetchWithAuth(`/user/${id}`, {
      method: 'DELETE'
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '删除用户失败');
    }
  }

  // 更新密码
  public static async updatePassword(id: string, newPassword: string): Promise<void> {
    const response = await fetchWithAuth(`/user/${id}/password`, {
      method: 'PUT',
      body: JSON.stringify({ NewPassword: newPassword })
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '更新密码失败');
    }
  }

  // 获取当前登录用户信息
  public static async getCurrentUser(): Promise<UserModel> {
    try {
      // 首先从token中获取用户信息，如果没有则尝试其他方式
      const token = localStorage.getItem('authToken');
      if (!token) {
        throw new Error('未登录');
      }

      // 尝试解析token获取userId或username
      // 注意：这里简化处理，实际项目中可能需要更复杂的token解析
      // 或者直接调用一个专门的接口获取当前用户信息
      
      // 这里假设token中包含userId信息，或者调用其他服务
      // 为了简化，我们可以先抛出一个未实现的错误，让调用者处理
      throw new Error('需要实现token解析或调用专用接口');
    } catch (error) {
      throw error;
    }
  }
}