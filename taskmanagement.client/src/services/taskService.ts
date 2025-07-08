import { apiService } from "./api";
import {
  Task,
  CreateTaskRequest,
  UpdateTaskRequest,
  PatchTaskRequest,
} from "../types/task";

export class TaskService {
  async getTasks(): Promise<Task[]> {
    return apiService.get<Task[]>("/tasks");
  }

  async getTask(id: number): Promise<Task> {
    return apiService.get<Task>(`/tasks/${id}`);
  }

  async createTask(taskData: CreateTaskRequest): Promise<Task> {
    return apiService.post<Task>("/tasks", taskData);
  }

  async updateTask(id: number, taskData: UpdateTaskRequest): Promise<Task> {
    return apiService.put<Task>(`/tasks/${id}`, taskData);
  }

  async patchTask(id: number, taskData: PatchTaskRequest): Promise<Task> {
    return apiService.patch<Task>(`/tasks/${id}`, taskData);
  }

  async deleteTask(id: number): Promise<void> {
    return apiService.delete<void>(`/tasks/${id}`);
  }
}

export const taskService = new TaskService();
