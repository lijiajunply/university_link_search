import { url } from "./UrlService";
import type { LinkModel } from "../models/link";

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

export class LinkService {
  // 获取所有链接
  public static async getAllLinks(): Promise<LinkModel[]> {
    const response = await fetch(`${url}/link`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '获取链接列表失败');
    }

    return await response.json();
  }

  // 根据ID获取链接
  public static async getLinkById(id: string): Promise<LinkModel> {
    const response = await fetch(`${url}/link/${id}`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '获取链接失败');
    }

    return await response.json();
  }

  // 根据Key获取链接
  public static async getLinkByKey(key: string): Promise<LinkModel> {
    const response = await fetch(`${url}/link/key/${key}`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '获取链接失败');
    }

    return await response.json();
  }

  // 根据分类获取链接
  public static async getLinksByCategory(categoryId: string): Promise<LinkModel[]> {
    const response = await fetch(`${url}/link/category/${categoryId}`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '获取分类下的链接失败');
    }

    return await response.json();
  }

  // 创建链接
  public static async createLink(link: LinkModel): Promise<LinkModel> {
    const response = await fetchWithAuth('/update', {
      method: 'POST',
      body: JSON.stringify(link)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '创建链接失败');
    }

    return await response.json();
  }

  // 更新链接
  public static async updateLink(link: LinkModel): Promise<void> {
    const response = await fetchWithAuth('/link', {
      method: 'POST',
      body: JSON.stringify(link)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '更新链接失败');
    }
  }

  // 删除链接
  public static async deleteLink(id: string): Promise<void> {
    const response = await fetchWithAuth(`/link/${id}`, {
      method: 'DELETE'
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '删除链接失败');
    }
  }

  // 更新链接排序
  public static async updateLinkSort(categoryId: string, linkIds: string[]): Promise<void> {
    const response = await fetchWithAuth(`/link/sort/${categoryId}`, {
      method: 'PUT',
      body: JSON.stringify(linkIds)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '更新链接排序失败');
    }
  }

  // 搜索链接
  public static async searchLinks(keyword: string): Promise<LinkModel[]> {
    const response = await fetch(`${url}/link/search?keyword=${encodeURIComponent(keyword)}`);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || '搜索链接失败');
    }

    return await response.json();
  }
}