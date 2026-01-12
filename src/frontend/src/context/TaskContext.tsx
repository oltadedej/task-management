import React, { createContext, useContext, useState, useCallback, ReactNode } from 'react';
import { TaskDto, CreateTaskDto, UpdateTaskDto, TaskStatus } from '../types/task.types';
import { apiService, ApiError } from '../services/api.service';
import { useNotification } from './NotificationContext';

/**
 * State interface for the task context.
 */
interface TaskContextState {
  tasks: TaskDto[];
  loading: boolean;
  error: string | null;
  selectedStatusFilter: TaskStatus | null;
}

/**
 * Actions interface for the task context.
 */
interface TaskContextActions {
  fetchTasks: () => Promise<void>;
  fetchTaskById: (id: string) => Promise<TaskDto | null>;
  fetchTasksByStatus: (status: TaskStatus) => Promise<void>;
  createTask: (task: CreateTaskDto) => Promise<TaskDto | null>;
  updateTask: (id: string, task: UpdateTaskDto) => Promise<TaskDto | null>;
  deleteTask: (id: string) => Promise<boolean>;
  markTaskComplete: (id: string) => Promise<boolean>;
  markTaskIncomplete: (id: string) => Promise<boolean>;
  markTaskInProgress: (id: string) => Promise<boolean>;
  setStatusFilter: (status: TaskStatus | null) => void;
  clearError: () => void;
}

/**
 * Combined context type.
 */
type TaskContextType = TaskContextState & TaskContextActions;

const TaskContext = createContext<TaskContextType | undefined>(undefined);

/**
 * Props for TaskProvider component.
 */
interface TaskProviderProps {
  children: ReactNode;
}

/**
 * Task context provider component.
 * Manages task state and provides actions for CRUD operations.
 */
export const TaskProvider: React.FC<TaskProviderProps> = ({ children }) => {
  const { showError, showSuccess } = useNotification();
  
  const [state, setState] = useState<TaskContextState>({
    tasks: [],
    loading: false,
    error: null,
    selectedStatusFilter: null,
  });

  const setLoading = useCallback((loading: boolean) => {
    setState((prev) => ({ ...prev, loading }));
  }, []);

  const setError = useCallback((error: string | null) => {
    setState((prev) => ({ ...prev, error }));
  }, []);

  /**
   * Fetch all tasks.
   */
  const fetchTasks = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const tasks = await apiService.getAllTasks();
      setState((prev) => ({ ...prev, tasks, loading: false }));
    } catch (err) {
      const error = err instanceof ApiError ? err.message : 'Failed to fetch tasks';
      setError(error);
      showError(error);
      setLoading(false);
    }
  }, [setLoading, setError, showError]);

  /**
   * Fetch a task by ID.
   */
  const fetchTaskById = useCallback(async (id: string): Promise<TaskDto | null> => {
    setLoading(true);
    setError(null);
    try {
      const task = await apiService.getTaskById(id);
      setLoading(false);
      return task;
    } catch (err) {
      const error = err instanceof ApiError ? err.message : 'Failed to fetch task';
      setError(error);
      showError(error);
      setLoading(false);
      return null;
    }
  }, [setLoading, setError, showError]);

  /**
   * Fetch tasks filtered by status.
   */
  const fetchTasksByStatus = useCallback(async (status: TaskStatus) => {
    setLoading(true);
    setError(null);
    try {
      const tasks = await apiService.getTasksByStatus(status);
      setState((prev) => ({
        ...prev,
        tasks,
        loading: false,
        selectedStatusFilter: status,
      }));
    } catch (err) {
      const error = err instanceof ApiError ? err.message : 'Failed to fetch tasks by status';
      setError(error);
      showError(error);
      setLoading(false);
    }
  }, [setLoading, setError, showError]);

  /**
   * Create a new task.
   */
  const createTask = useCallback(async (task: CreateTaskDto): Promise<TaskDto | null> => {
    setLoading(true);
    setError(null);
    try {
      const newTask = await apiService.createTask(task);
      setState((prev) => ({
        ...prev,
        tasks: [newTask, ...prev.tasks],
        loading: false,
      }));
      showSuccess('Task created successfully!');
      return newTask;
    } catch (err) {
      const error = err instanceof ApiError ? err.message : 'Failed to create task';
      setError(error);
      showError(error);
      setLoading(false);
      return null;
    }
  }, [setLoading, setError, showError, showSuccess]);

  /**
   * Update an existing task.
   */
  const updateTask = useCallback(async (
    id: string,
    task: UpdateTaskDto
  ): Promise<TaskDto | null> => {
    setLoading(true);
    setError(null);
    try {
      const updatedTask = await apiService.updateTask(id, task);
      setState((prev) => ({
        ...prev,
        tasks: prev.tasks.map((t) => (t.id === id ? updatedTask : t)),
        loading: false,
      }));
      showSuccess('Task updated successfully!');
      return updatedTask;
    } catch (err) {
      const error = err instanceof ApiError ? err.message : 'Failed to update task';
      setError(error);
      showError(error);
      setLoading(false);
      return null;
    }
  }, [setLoading, setError, showError, showSuccess]);

  /**
   * Delete a task.
   */
  const deleteTask = useCallback(async (id: string): Promise<boolean> => {
    setLoading(true);
    setError(null);
    try {
      await apiService.deleteTask(id);
      setState((prev) => ({
        ...prev,
        tasks: prev.tasks.filter((t) => t.id !== id),
        loading: false,
      }));
      showSuccess('Task deleted successfully!');
      return true;
    } catch (err) {
      const error = err instanceof ApiError ? err.message : 'Failed to delete task';
      setError(error);
      showError(error);
      setLoading(false);
      return false;
    }
  }, [setLoading, setError, showError, showSuccess]);

  /**
   * Mark a task as complete.
   */
  const markTaskComplete = useCallback(async (id: string): Promise<boolean> => {
    setLoading(true);
    setError(null);
    try {
      const updatedTask = await apiService.markTaskComplete(id);
      setState((prev) => ({
        ...prev,
        tasks: prev.tasks.map((t) => (t.id === id ? updatedTask : t)),
        loading: false,
      }));
      showSuccess('Task marked as complete!');
      return true;
    } catch (err) {
      const error = err instanceof ApiError ? err.message : 'Failed to mark task as complete';
      setError(error);
      showError(error);
      setLoading(false);
      return false;
    }
  }, [setLoading, setError, showError, showSuccess]);

  /**
   * Mark a task as incomplete (Not Started).
   */
  const markTaskIncomplete = useCallback(async (id: string): Promise<boolean> => {
    setLoading(true);
    setError(null);
    try {
      const updatedTask = await apiService.markTaskIncomplete(id);
      setState((prev) => ({
        ...prev,
        tasks: prev.tasks.map((t) => (t.id === id ? updatedTask : t)),
        loading: false,
      }));
      showSuccess('Task marked as incomplete!');
      return true;
    } catch (err) {
      const error = err instanceof ApiError ? err.message : 'Failed to mark task as incomplete';
      setError(error);
      showError(error);
      setLoading(false);
      return false;
    }
  }, [setLoading, setError, showError, showSuccess]);

  /**
   * Mark a task as in progress.
   */
  const markTaskInProgress = useCallback(async (id: string): Promise<boolean> => {
    setLoading(true);
    setError(null);
    try {
      const updatedTask = await apiService.markTaskInProgress(id);
      setState((prev) => ({
        ...prev,
        tasks: prev.tasks.map((t) => (t.id === id ? updatedTask : t)),
        loading: false,
      }));
      showSuccess('Task marked as in progress!');
      return true;
    } catch (err) {
      const error = err instanceof ApiError ? err.message : 'Failed to mark task as in progress';
      setError(error);
      showError(error);
      setLoading(false);
      return false;
    }
  }, [setLoading, setError, showError, showSuccess]);

  /**
   * Set the status filter.
   */
  const setStatusFilter = useCallback((status: TaskStatus | null) => {
    setState((prev) => ({ ...prev, selectedStatusFilter: status }));
  }, []);

  /**
   * Clear the current error.
   */
  const clearError = useCallback(() => {
    setError(null);
  }, [setError]);

  const value: TaskContextType = {
    ...state,
    fetchTasks,
    fetchTaskById,
    fetchTasksByStatus,
    createTask,
    updateTask,
    deleteTask,
    markTaskComplete,
    markTaskIncomplete,
    markTaskInProgress,
    setStatusFilter,
    clearError,
  };

  return <TaskContext.Provider value={value}>{children}</TaskContext.Provider>;
};

/**
 * Hook to use the task context.
 * Must be used within a TaskProvider.
 */
export const useTaskContext = (): TaskContextType => {
  const context = useContext(TaskContext);
  if (context === undefined) {
    throw new Error('useTaskContext must be used within a TaskProvider');
  }
  return context;
};

