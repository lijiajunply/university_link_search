import { url } from "./UrlService";

// 通用请求方法，处理认证头
async function fetchWithAuth(endpoint: string, options: RequestInit = {}) {
  const token = localStorage.getItem('authToken');
  const headers: Record<string, string> = {
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

export class DataService {
  // 下载数据
  // 根据用户角色，返回不同格式的数据：
  // - 普通成员：获取Markdown格式的链接数据
  // - 管理员：获取完整的JSON数据
  public static async downloadData(): Promise<void> {
    try {
      const response = await fetchWithAuth('/data/download');

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || '下载数据失败');
      }

      // 获取响应头中的文件名
      const contentDisposition = response.headers.get('Content-Disposition');
      let fileName = 'university_links';
      
      if (contentDisposition) {
        const matches = contentDisposition.match(/filename="([^"]+)"/);
        if (matches && matches.length > 1) {
          fileName = matches[1];
        }
      }

      // 获取Blob数据
      const blob = await response.blob();
      
      // 创建下载链接并触发下载
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      throw error;
    }
  }

  // 上传数据
  // 仅Founder/Manager角色可上传JSON数据并导入分类和链接
  public static async uploadData(file: File): Promise<{ success: boolean; message: string }> {
    try {
      // 创建FormData对象
      const formData = new FormData();
      formData.append('file', file);

      // 注意：上传文件时不设置Content-Type为application/json，让浏览器自动设置
      const response = await fetchWithAuth('/data/upload', {
        method: 'POST',
        headers: {}, // 不设置Content-Type
        body: formData
      });

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || '上传数据失败');
      }

      return await response.json();
    } catch (error) {
      throw error;
    }
  }

  // 验证文件是否为有效的JSON文件
  public static isValidJsonFile(file: File): boolean {
    return file.type === 'application/json' || file.name.endsWith('.json');
  }

  // 读取并验证JSON文件内容
  public static async readAndValidateJsonFile(file: File): Promise<boolean> {
    try {
      const content = await file.text();
      JSON.parse(content); // 尝试解析JSON
      return true;
    } catch (error) {
      return false;
    }
  }
}