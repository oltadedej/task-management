import React from 'react';
import { TaskDto, TaskStatus, TaskStatusMap } from '../types/task.types';
import { format } from 'date-fns';
import { useTaskContext } from '../context/TaskContext';

/**
 * Props for TaskCard component.
 */
interface TaskCardProps {
  task: TaskDto;
  onEdit?: (task: TaskDto) => void;
}

/**
 * Task card component displaying individual task information.
 * Accessible and responsive design.
 */
export const TaskCard: React.FC<TaskCardProps> = ({ task, onEdit }) => {
  const { markTaskComplete, markTaskIncomplete, markTaskInProgress, deleteTask } = useTaskContext();
  const statusInfo = TaskStatusMap[task.status];
  const dueDate = task.dueDate ? new Date(task.dueDate) : null;
  const isOverdue = dueDate && dueDate < new Date() && task.status !== TaskStatus.Completed;

  const handleToggleComplete = async () => {
    if (task.status === TaskStatus.Completed) {
      await markTaskIncomplete(task.id);
    } else {
      await markTaskComplete(task.id);
    }
  };

  const handleMarkInProgress = async () => {
    await markTaskInProgress(task.id);
  };

  const handleDelete = async () => {
    if (window.confirm('Are you sure you want to delete this task?')) {
      await deleteTask(task.id);
    }
  };

  return (
    <div
      className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow duration-200 border border-gray-200"
      role="article"
      aria-label={`Task: ${task.title}`}
    >
      <div className="flex items-start justify-between mb-4">
        <div className="flex-1">
          <h3 className="text-lg font-semibold text-gray-900 mb-2">{task.title}</h3>
          {task.description && (
            <p className="text-gray-600 text-sm mb-3 line-clamp-2">{task.description}</p>
          )}
        </div>
        <div className="flex flex-col items-end gap-2 ml-4">
          <span
            className={`px-3 py-1 rounded-full text-xs font-medium ${statusInfo.bgColor} ${statusInfo.color}`}
            aria-label={`Status: ${statusInfo.label}`}
          >
            {statusInfo.label}
          </span>
          {isOverdue && (
            <span className="px-2 py-1 rounded text-xs font-medium bg-danger-100 text-danger-700">
              Overdue
            </span>
          )}
        </div>
      </div>

      <div className="flex flex-wrap gap-2 text-xs text-gray-500 mb-4">
        {dueDate && (
          <div className="flex items-center gap-1">
            <svg
              className="w-4 h-4"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              aria-hidden="true"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
              />
            </svg>
            <span className={isOverdue ? 'text-danger-600 font-medium' : ''}>
              Due: {format(dueDate, 'MMM dd, yyyy')}
            </span>
          </div>
        )}
        <div className="flex items-center gap-1">
          <svg
            className="w-4 h-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <span>Created: {format(new Date(task.createdAt), 'MMM dd, yyyy')}</span>
        </div>
      </div>

      <div className="flex flex-wrap gap-2 pt-4 border-t border-gray-200">
        {task.status !== TaskStatus.InProgress && (
          <button
            onClick={handleMarkInProgress}
            className="px-4 py-2 rounded-md text-sm font-medium bg-primary-100 text-primary-700 hover:bg-primary-200 transition-colors duration-200"
            aria-label="Mark task as in progress"
          >
            Mark In Progress
          </button>
        )}
        <button
          onClick={handleToggleComplete}
          className={`px-4 py-2 rounded-md text-sm font-medium transition-colors duration-200 ${
            task.status === TaskStatus.Completed
              ? 'bg-warning-100 text-warning-700 hover:bg-warning-200'
              : 'bg-success-100 text-success-700 hover:bg-success-200'
          }`}
          aria-label={task.status === TaskStatus.Completed ? 'Mark as incomplete' : 'Mark as complete'}
        >
          {task.status === TaskStatus.Completed ? 'Mark Incomplete' : 'Mark Complete'}
        </button>
        {onEdit && (
          <button
            onClick={() => onEdit(task)}
            className="px-4 py-2 rounded-md text-sm font-medium bg-primary-100 text-primary-700 hover:bg-primary-200 transition-colors duration-200"
            aria-label={`Edit task: ${task.title}`}
          >
            Edit
          </button>
        )}
        <button
          onClick={handleDelete}
          className="px-4 py-2 rounded-md text-sm font-medium bg-danger-100 text-danger-700 hover:bg-danger-200 transition-colors duration-200"
          aria-label={`Delete task: ${task.title}`}
        >
          Delete
        </button>
      </div>
    </div>
  );
};

