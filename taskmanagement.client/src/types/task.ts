export type TaskStatus = 'Pending' | 'InProgress' | 'Completed';

export const TaskStatus = {
    PENDING: 'Pending' as TaskStatus,
    IN_PROGRESS: 'InProgress' as TaskStatus,
    COMPLETED: 'Completed' as TaskStatus
};

export interface Task {
    id: number;
    title: string;
    description: string;
    status: TaskStatus;
    dueDate: string;
    userId: number;
    createdAt: string;
    updatedAt?: string;
}

export interface CreateTaskRequest {
    title: string;
    description: string;
    status: number; // 0=Pending, 1=InProgress, 2=Completed
    dueDate: string;
}

export interface UpdateTaskRequest {
    title: string;
    description: string;
    status: number;
    dueDate: string;
}

export interface PatchTaskRequest {
    title?: string;
    description?: string;
    status?: number;
    dueDate?: string;
}

export interface TaskState {
    tasks: Task[];
    isLoading: boolean;
    error: string | null;
    filters: TaskFilters;
}

export interface TaskFilters {
    status: TaskStatus | 'All';
    search: string;
}

export type TaskAction =
    | { type: 'TASKS_LOADING' }
    | { type: 'TASKS_SUCCESS'; payload: Task[] }
    | { type: 'TASKS_FAILURE'; payload: string }
    | { type: 'TASK_ADD'; payload: Task }
    | { type: 'TASK_UPDATE'; payload: Task }
    | { type: 'TASK_DELETE'; payload: number }
    | { type: 'TASKS_SET_FILTER'; payload: Partial<TaskFilters> }
    | { type: 'TASKS_CLEAR_ERROR' };