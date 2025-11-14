import {defineStore} from 'pinia';
import {LoginService} from "../services/LoginService.ts";
import {type LoginRequest} from "../models/auth.ts";

export const useAuthorizationStore = defineStore('AuthorizationId', {
    state: () => ({
        Authorization: localStorage.getItem('Authorization') || ''
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
        }
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
        }
    }
});