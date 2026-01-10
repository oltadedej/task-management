import React, { useState, useEffect } from 'react';
import { TaskDto, TaskStatus } from '../types/task.types';
import { TaskList } from '../components/TaskList';
import { FilterBar } from '../components/FilterBar';
import { TaskForm } from '../components/TaskForm';
import { useTaskContext } from '../context/TaskContext';

/**
 * Home page component displaying the main task management interface.
 * Accessible and responsive design.
 */
export const Home: React.FC = () => {
  const { tasks, loading, error, fetchTasks, clearError } = useTaskContext();
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingTask, setEditingTask] = useState<TaskDto | null>(null);

  useEffect(() => {
    fetchTasks();
  }, [fetchTasks]);

  const handleOpenForm = () => {
    setEditingTask(null);
    setIsFormOpen(true);
  };

  const handleEditTask = (task: TaskDto) => {
    setEditingTask(task);
    setIsFormOpen(true);
  };

  const handleCloseForm = () => {
    setIsFormOpen(false);
    setEditingTask(null);
  };

  const handleFormSuccess = () => {
    // Refresh tasks after successful create/update
    fetchTasks();
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b border-gray-200 sticky top-0 z-40">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-primary-600 rounded-lg flex items-center justify-center">
                <svg
                  className="w-6 h-6 text-white"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                  aria-hidden="true"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4"
                  />
                </svg>
              </div>
              <div>
                <h1 className="text-2xl font-bold text-gray-900">Task Management</h1>
                <p className="text-sm text-gray-500">Organize and manage your tasks efficiently</p>
              </div>
            </div>
            <button
              onClick={handleOpenForm}
              className="px-6 py-2 bg-primary-600 text-white rounded-md font-medium hover:bg-primary-700 transition-colors shadow-md hover:shadow-lg focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2"
              aria-label="Create new task"
            >
              <span className="flex items-center gap-2">
                <svg
                  className="w-5 h-5"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                  aria-hidden="true"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M12 4v16m8-8H4"
                  />
                </svg>
                New Task
              </span>
            </button>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Error Alert */}
        {error && (
          <div
            className="mb-6 bg-danger-50 border border-danger-200 text-danger-700 px-4 py-3 rounded-md flex items-center justify-between"
            role="alert"
            aria-live="polite"
          >
            <div className="flex items-center gap-2">
              <svg
                className="w-5 h-5"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
                aria-hidden="true"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              <span>{error}</span>
            </div>
            <button
              onClick={clearError}
              className="text-danger-700 hover:text-danger-900 transition-colors"
              aria-label="Dismiss error"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            </button>
          </div>
        )}

        {/* Filter Bar */}
        <FilterBar />

        {/* Loading State */}
        {loading && (
          <div className="flex items-center justify-center py-12" role="status" aria-live="polite">
            <div className="flex flex-col items-center gap-4">
              <div className="w-12 h-12 border-4 border-primary-200 border-t-primary-600 rounded-full animate-spin" />
              <p className="text-gray-600 font-medium">Loading tasks...</p>
            </div>
          </div>
        )}

        {/* Task List */}
        {!loading && (
          <div className="mb-8">
            <TaskList tasks={tasks} onEdit={handleEditTask} />
          </div>
        )}

        {/* Stats */}
        {!loading && tasks.length > 0 && (
          <div className="mt-8 grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div className="bg-white rounded-lg shadow-md p-4">
              <div className="text-sm font-medium text-gray-500">Total Tasks</div>
              <div className="mt-2 text-3xl font-bold text-gray-900">{tasks.length}</div>
            </div>
            <div className="bg-white rounded-lg shadow-md p-4">
              <div className="text-sm font-medium text-gray-500">In Progress</div>
              <div className="mt-2 text-3xl font-bold text-primary-600">
                {tasks.filter((t) => t.status === TaskStatus.InProgress).length}
              </div>
            </div>
            <div className="bg-white rounded-lg shadow-md p-4">
              <div className="text-sm font-medium text-gray-500">Completed</div>
              <div className="mt-2 text-3xl font-bold text-success-600">
                {tasks.filter((t) => t.status === TaskStatus.Completed).length}
              </div>
            </div>
          </div>
        )}
      </main>

      {/* Task Form Modal */}
      {isFormOpen && (
        <TaskForm
          task={editingTask}
          onClose={handleCloseForm}
          onSuccess={handleFormSuccess}
        />
      )}
    </div>
  );
};

