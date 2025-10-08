import {url} from "./UrlService.ts";

export interface UserModel {
    userName: string;
    gender: string;
}

export interface LoginModel {
    username: string;
    password: string;
}

export class LoginService {
    public static async login(username: string, password: string): Promise<any> {
        return await fetch(`${url}/Login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                name: username,
                password: password
            })
        }).then(res => res.json())
    }
}