import { apiService } from './api';
import type { LoginRequest, RegisterRequest, LoginResponse, User } from '../types/auth';

export class AuthService {
    async login(credentials: LoginRequest): Promise<LoginResponse> {
        const response = await apiService.post<LoginResponse>('/auth/login', credentials);

        // Store token and user in localStorage
        localStorage.setItem('authToken', response.token);
        localStorage.setItem('authUser', JSON.stringify(response.user));

        return response;
    }

    async register(userData: RegisterRequest): Promise<User> {
        return apiService.post<User>('/auth/register', userData);
    }

    async getProfile(): Promise<User> {
        return apiService.get<User>('/auth/profile');
    }

    logout(): void {
        localStorage.removeItem('authToken');
        localStorage.removeItem('authUser');
    }

    getStoredToken(): string | null {
        return localStorage.getItem('authToken');
    }

    getStoredUser(): User | null {
        const userStr = localStorage.getItem('authUser');
        if (!userStr) return null;

        try {
            return JSON.parse(userStr);
        } catch {
            return null;
        }
    }

    isTokenExpired(token: string): boolean {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const currentTime = Date.now() / 1000;
            return payload.exp < currentTime;
        } catch {
            return true;
        }
    }
}

export const authService = new AuthService();