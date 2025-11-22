import { url } from "./UrlService";
import type { CategoryModel } from "../models/category";

// 通用请求方法，处理认证头
async function fetchWithAuth(endpoint: string, options: RequestInit = {}) {
  const token = localStorage.getItem('Authorization');
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

export class CategoryService {
  // 获取所有分类
  public static async getAllCategories(): Promise<CategoryModel[]> {
    const response = await fetch(`${url}/category`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '获取分类列表失败');
    }

    return await response.json();
  }

  // 根据ID获取分类
  public static async getCategoryById(id: string): Promise<CategoryModel> {
    const response = await fetch(`${url}/category/${id}`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '获取分类失败');
    }

    return await response.json();
  }

  // 创建分类
  public static async createCategory(category: CategoryModel): Promise<CategoryModel> {
    const response = await fetchWithAuth('/category', {
      method: 'POST',
      body: JSON.stringify(category)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '创建分类失败');
    }

    return await response.json();
  }

  // 更新分类
  public static async updateCategory(category: CategoryModel): Promise<void> {
    const response = await fetchWithAuth('/update', {
      method: 'POST',
      body: JSON.stringify(category)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '更新分类失败');
    }
  }

  // 删除分类
  public static async deleteCategory(id: string): Promise<void> {
    const response = await fetchWithAuth(`/category/${id}`, {
      method: 'DELETE'
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '删除分类失败');
    }
  }

  // 更新分类排序
  public static async updateCategorySort(categoryIds: string[]): Promise<void> {
    const response = await fetchWithAuth('/category/sort', {
      method: 'PUT',
      body: JSON.stringify(categoryIds)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '更新分类排序失败');
    }
  }

  // 搜索分类
  public static async searchCategories(keyword: string): Promise<CategoryModel[]> {
    const response = await fetch(`${url}/category/search?keyword=${encodeURIComponent(keyword)}`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '搜索分类失败');
    }

    return await response.json();
  }
}