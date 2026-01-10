import React from 'react';
import { TaskDto } from '../types/task.types';
import { TaskCard } from './TaskCard';

/**
 * Props for TaskList component.
 */
interface TaskListProps {
  tasks: TaskDto[];
  onEdit?: (task: TaskDto) => void;
  emptyMessage?: string;
}

/**
 * Task list component displaying a collection of tasks.
 * Accessible and responsive design.
 */
export const TaskList: React.FC<TaskListProps> = ({
  tasks,
  onEdit,
  emptyMessage = 'No tasks found. Create your first task to get started!',
}) => {
  if (tasks.length === 0) {
    return (
      <div
        className="text-center py-12 px-4 bg-gray-50 rounded-lg border-2 border-dashed border-gray-300"
        role="status"
        aria-live="polite"
      >
        <svg
          className="mx-auto h-12 w-12 text-gray-400"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
          aria-hidden="true"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={2}
            d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"
          />
        </svg>
        <h3 className="mt-2 text-sm font-medium text-gray-900">No tasks</h3>
        <p className="mt-1 text-sm text-gray-500">{emptyMessage}</p>
      </div>
    );
  }

  return (
    <div className="space-y-4" role="list" aria-label="Task list">
      {tasks.map((task) => (
        <div key={task.id} role="listitem">
          <TaskCard task={task} onEdit={onEdit} />
        </div>
      ))}
    </div>
  );
};

