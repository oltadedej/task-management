/**
 * Task status enum matching the backend.
 */
export enum TaskStatus {
  NotStarted = 0,
  InProgress = 1,
  Completed = 2,
}

/**
 * Task data transfer object matching the backend.
 */
export interface TaskDto {
  id: string;
  title: string;
  description?: string;
  status: TaskStatus;
  createdAt: string;
  updatedAt: string;
  dueDate?: string;
}

/**
 * DTO for creating a new task.
 */
export interface CreateTaskDto {
  title: string;
  description?: string;
  dueDate?: string;
}

/**
 * DTO for updating an existing task.
 */
export interface UpdateTaskDto {
  title: string;
  description?: string;
  dueDate?: string;
}

/**
 * Task status display information.
 */
export interface TaskStatusInfo {
  label: string;
  color: string;
  bgColor: string;
}

/**
 * Mapping of task status to display information.
 */
export const TaskStatusMap: Record<TaskStatus, TaskStatusInfo> = {
  [TaskStatus.NotStarted]: {
    label: 'Not Started',
    color: 'text-gray-700',
    bgColor: 'bg-gray-100',
  },
  [TaskStatus.InProgress]: {
    label: 'In Progress',
    color: 'text-primary-700',
    bgColor: 'bg-primary-100',
  },
  [TaskStatus.Completed]: {
    label: 'Completed',
    color: 'text-success-700',
    bgColor: 'bg-success-100',
  },
};

