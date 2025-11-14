// 分类相关的数据模型定义
import type { LinkModel } from './link';

export interface CategoryModel {
  key: string;
  name: string;
  description?: string;
  icon: string;
  index: number;
  links: LinkModel[];
}