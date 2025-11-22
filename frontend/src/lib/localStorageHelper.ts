/**
 * localStorage 辅助类，提供简单的存储和读取功能
 */
class LocalStorageHelper {
  /**
   * 从localStorage获取值
   * @param key 存储的键名
   * @returns 存储的值，如果不存在或localStorage不可用则返回null
   */
  static get(key: string): string | null {
    try {
      return localStorage.getItem(key);
    } catch (error) {
      console.error('获取localStorage失败:', error);
      return null;
    }
  }

  /**
   * 向localStorage存储值
   * @param key 存储的键名
   * @param value 要存储的值
   * @returns 是否存储成功
   */
  static set(key: string, value: string): boolean {
    try {
      localStorage.setItem(key, value);
      return true;
    } catch (error) {
      console.error('设置localStorage失败:', error);
      return false;
    }
  }

  /**
   * 从localStorage删除值
   * @param key 要删除的键名
   * @returns 是否删除成功
   */
  static remove(key: string): boolean {
    try {
      localStorage.removeItem(key);
      return true;
    } catch (error) {
      console.error('删除localStorage失败:', error);
      return false;
    }
  }

  /**
   * 清空localStorage
   * @returns 是否清空成功
   */
  static clear(): boolean {
    try {
      localStorage.clear();
      return true;
    } catch (error) {
      console.error('清空localStorage失败:', error);
      return false;
    }
  }
}

export default LocalStorageHelper;