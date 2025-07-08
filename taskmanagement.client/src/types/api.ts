export interface ApiResponse<T> {
    data?: T;
    error?: string;
    success: boolean;
}

export interface ApiError {
    error: string;
    errors?: Array<{
        property: string;
        message: string;
    }>;
    timestamp: string;
}

export interface PaginatedResponse<T> {
    data: T[];
    total: number;
    page: number;
    pageSize: number;
}