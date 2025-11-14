import { AuthService } from "./AuthService";
import type { LoginRequest, TokenResponse } from "../models/auth";

export class LoginService {
    // 兼容旧接口，但内部使用新的AuthService
    public static async login(username: string, password: string): Promise<TokenResponse> {
        const loginRequest: LoginRequest = {
            username,
            password
        };
        return await AuthService.login(loginRequest);
    }
}