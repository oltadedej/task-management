import React from 'react';
import { TaskStatus, TaskStatusMap } from '../types/task.types';
import { useTaskContext } from '../context/TaskContext';

/**
 * Filter bar component for filtering tasks by status.
 * Accessible and responsive design.
 */
export const FilterBar: React.FC = () => {
  const { selectedStatusFilter, setStatusFilter, fetchTasks, fetchTasksByStatus } = useTaskContext();

  const handleFilterChange = async (status: TaskStatus | null) => {
    setStatusFilter(status);
    if (status === null) {
      await fetchTasks();
    } else {
      await fetchTasksByStatus(status);
    }
  };

  const filterOptions: Array<{ value: TaskStatus | null; label: string }> = [
    { value: null, label: 'All Tasks' },
    { value: TaskStatus.NotStarted, label: TaskStatusMap[TaskStatus.NotStarted].label },
    { value: TaskStatus.InProgress, label: TaskStatusMap[TaskStatus.InProgress].label },
    { value: TaskStatus.Completed, label: TaskStatusMap[TaskStatus.Completed].label },
  ];

  return (
    <div className="bg-white rounded-lg shadow-md p-4 mb-6" role="toolbar" aria-label="Task filters">
      <div className="flex flex-wrap items-center gap-3">
        <span className="text-sm font-medium text-gray-700">Filter by status:</span>
        <div className="flex flex-wrap gap-2">
          {filterOptions.map((option) => {
            const isSelected = selectedStatusFilter === option.value;
            return (
              <button
                key={option.value ?? 'all'}
                onClick={() => handleFilterChange(option.value)}
                className={`px-4 py-2 rounded-md text-sm font-medium transition-all duration-200 ${
                  isSelected
                    ? 'bg-primary-600 text-white shadow-md'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
                aria-pressed={isSelected}
                aria-label={`Filter by ${option.label}`}
              >
                {option.label}
              </button>
            );
          })}
        </div>
      </div>
    </div>
  );
};

