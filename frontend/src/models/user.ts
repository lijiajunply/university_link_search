// 用户相关的数据模型定义
export interface UserModel {
  userName: string;
  userId: string;
  gender: string;
  className: string;
  identity: string;
}

export interface LoginModel {
  name: string;
  id: string;
}

// 密码更新模型
export interface PasswordUpdateModel {
  oldPassword: string;
  newPassword: string;
}