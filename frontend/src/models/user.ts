// 用户相关的数据模型定义
export interface UserModel {
  userName: string;
  userId: string;
  className: string;
  identity: string;
}

// 密码更新模型
export interface PasswordUpdateModel {
  oldPassword: string;
  newPassword: string;
}