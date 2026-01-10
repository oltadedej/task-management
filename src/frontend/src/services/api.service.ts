import axios, { AxiosInstance, AxiosError } from 'axios';
import { TaskDto, CreateTaskDto, UpdateTaskDto, TaskStatus } from '../types/task.types';

/**
 * Configuration for the API service.
 * The base URL can be overridden via environment variables.
 */
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7027/api/v1';

/**
 * Custom error class for API errors.
 */
export class ApiError extends Error {
  constructor(
    message: string,
    public status?: number,
    public data?: unknown
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

/**
 * API service for interacting with the backend Task Management API.
 */
class ApiService {
  private axiosInstance: AxiosInstance;

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
      timeout: 10000,
    });

    // Request interceptor for logging/debugging
    this.axiosInstance.interceptors.request.use(
      (config) => {
        console.debug(`[API Request] ${config.method?.toUpperCase()} ${config.url}`);
        return config;
      },
      (error) => {
        console.error('[API Request Error]', error);
        return Promise.reject(error);
      }
    );

    // Response interceptor for error handling
    this.axiosInstance.interceptors.response.use(
      (response) => {
        console.debug(`[API Response] ${response.status} ${response.config.url}`);
        return response;
      },
      (error: AxiosError) => {
        console.error('[API Response Error]', error);
        const apiError = new ApiError(
          error.response?.data?.message || error.message || 'An unexpected error occurred',
          error.response?.status,
          error.response?.data
        );
        return Promise.reject(apiError);
      }
    );
  }

  /**
   * Get all tasks.
   */
  async getAllTasks(): Promise<TaskDto[]> {
    const response = await this.axiosInstance.get<TaskDto[]>('/tasks');
    return response.data;
  }

  /**
   * Get a task by ID.
   */
  async getTaskById(id: string): Promise<TaskDto> {
    const response = await this.axiosInstance.get<TaskDto>(`/tasks/${id}`);
    return response.data;
  }

  /**
   * Get tasks filtered by status.
   */
  async getTasksByStatus(status: TaskStatus): Promise<TaskDto[]> {
    const response = await this.axiosInstance.get<TaskDto[]>(`/tasks/status/${status}`);
    return response.data;
  }

  /**
   * Create a new task.
   */
  async createTask(task: CreateTaskDto): Promise<TaskDto> {
    const response = await this.axiosInstance.post<TaskDto>('/tasks', task);
    return response.data;
  }

  /**
   * Update an existing task.
   */
  async updateTask(id: string, task: UpdateTaskDto): Promise<TaskDto> {
    const response = await this.axiosInstance.put<TaskDto>(`/tasks/${id}`, task);
    return response.data;
  }

  /**
   * Delete a task.
   */
  async deleteTask(id: string): Promise<void> {
    await this.axiosInstance.delete(`/tasks/${id}`);
  }

  /**
   * Mark a task as complete.
   */
  async markTaskComplete(id: string): Promise<TaskDto> {
    const response = await this.axiosInstance.patch<TaskDto>(`/tasks/${id}/complete`);
    return response.data;
  }

  /**
   * Mark a task as incomplete (Not Started).
   */
  async markTaskIncomplete(id: string): Promise<TaskDto> {
    const response = await this.axiosInstance.patch<TaskDto>(`/tasks/${id}/incomplete`);
    return response.data;
  }
}

// Export a singleton instance
export const apiService = new ApiService();

