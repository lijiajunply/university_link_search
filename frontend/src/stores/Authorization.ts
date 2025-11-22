import {defineStore} from 'pinia';
import {eduLoginService} from '../services/EduLoginService';
import type {ClubData} from "../models/ClubData.ts";

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
        // 添加edu相关的getter
        getIsShowEdu: (state) => state.isShowEdu,
        getEduPassword: (state) => state.eduPassword,
        getUserData: (state) => state.userData
    },
    actions: {
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
        setClubData(data: ClubData) {
            localStorage.setItem('club-data', JSON.stringify(data));
            localStorage.setItem('Authorization', data.token);
        },
        async handleGetEduData() {
            if (this.userData && this.userData.UserId) {
                return  await eduLoginService(this.userData.UserId, this.eduPassword);
            }
            return false;
        }
    }
});