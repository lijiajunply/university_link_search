import {defineStore} from 'pinia';
import {LoginService} from "../services/LoginService.ts";
import {type LoginRequest} from "../models/auth.ts";
import {eduLoginService} from '../services/EduLoginService.js';

export const useAuthorizationStore = defineStore('AuthorizationId', {
    state: () => ({
        Authorization: localStorage.getItem('Authorization') || '',
        // 添加edu相关状态
        isShowEdu: localStorage.getItem('is-show-edu') === 'true',
        eduPassword: localStorage.getItem('edu-password') || '',
        userData: localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user') || '{}') : null
    }),
    getters: {
        getAuthorization: (state) => state.Authorization,
        isAuthenticated: (state) => !!state.Authorization,
        getAuthorizationInfo: (state) :any => {
            let strings = state.Authorization.split('.'); // 确保是 JWT 格式
            if (strings.length !== 3) return null; // 简单校验是否为合法 JWT
            try {
                const payload = strings[1].replace(/-/g, '+').replace(/_/g, '/');
                const decodedPayload = atob(payload);
                const jsonPayload = decodeURIComponent(encodeURIComponent(decodedPayload));
                return JSON.parse(jsonPayload);
            } catch (e) {
                console.error('解析 Authorization 失败:', e);
                return null;
            }
        },
        getRole: (state) =>{
            let strings = state.Authorization.split('.'); // 确保是 JWT 格式
            if (strings.length !== 3) return null; // 简单校验是否为合法 JWT
            try {
                const payload = strings[1].replace(/-/g, '+').replace(/_/g, '/');
                const decodedPayload = atob(payload);
                const jsonPayload = decodeURIComponent(encodeURIComponent(decodedPayload));
                const json = JSON.parse(jsonPayload);
                return json['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
            } catch (e) {
                console.error('解析 Authorization 失败:', e);
                return null;
            }
        },
        // 添加edu相关的getter
        getIsShowEdu: (state) => state.isShowEdu,
        getEduPassword: (state) => state.eduPassword,
        getUserData: (state) => state.userData
    },
    actions: {
        // 修改token，并将token存入localStorage
        async login(user: LoginRequest): Promise<boolean> {
            try {
                const a = await LoginService.login(user.username, user.password)
                if (!a) {
                    return false;
                }
                this.Authorization = a.accessToken;
                localStorage.setItem('Authorization', a.accessToken);
                return true;
            } catch (e) {
                return false;
            }
        },
        logout() {
            this.Authorization = '';
            localStorage.removeItem('Authorization');
            // 清除edu相关数据
            this.isShowEdu = false;
            this.eduPassword = '';
            this.userData = null;
            localStorage.removeItem('is-show-edu');
            localStorage.removeItem('edu-password');
            localStorage.removeItem('user');
        },
        // 添加edu相关的action
        setIsShowEdu(value: boolean) {
            this.isShowEdu = value;
            localStorage.setItem('is-show-edu', value.toString());
        },
        setEduPassword(password: string) {
            this.eduPassword = password;
            localStorage.setItem('edu-password', password);
        },
        setUserData(user: any) {
            this.userData = user;
            localStorage.setItem('user', JSON.stringify(user));
        },
        async handleGetEduData() {
            if (this.userData && this.userData.UserId) {
                return await eduLoginService(this.userData.UserId, this.eduPassword);
            }
            return false;
        }
    }
});