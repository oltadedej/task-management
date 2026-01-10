import React, { useState, useEffect } from 'react';
import { TaskDto, CreateTaskDto, UpdateTaskDto } from '../types/task.types';
import { useTaskContext } from '../context/TaskContext';

/**
 * Props for TaskForm component.
 */
interface TaskFormProps {
  task?: TaskDto | null;
  onClose: () => void;
  onSuccess?: () => void;
}

/**
 * Task form component for creating and editing tasks.
 * Accessible and responsive design with validation.
 */
export const TaskForm: React.FC<TaskFormProps> = ({ task, onClose, onSuccess }) => {
  const { createTask, updateTask } = useTaskContext();
  const isEditing = !!task;

  const [formData, setFormData] = useState<CreateTaskDto | UpdateTaskDto>({
    title: task?.title || '',
    description: task?.description || '',
    dueDate: task?.dueDate ? task.dueDate.split('T')[0] : '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (task) {
      setFormData({
        title: task.title,
        description: task.description || '',
        dueDate: task.dueDate ? task.dueDate.split('T')[0] : '',
      });
    }
  }, [task]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.title.trim()) {
      newErrors.title = 'Title is required';
    } else if (formData.title.length > 200) {
      newErrors.title = 'Title must be 200 characters or less';
    }

    if (formData.description && formData.description.length > 1000) {
      newErrors.description = 'Description must be 1000 characters or less';
    }

    if (formData.dueDate) {
      const dueDate = new Date(formData.dueDate);
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      if (dueDate < today) {
        newErrors.dueDate = 'Due date cannot be in the past';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) {
      return;
    }

    setIsSubmitting(true);

    try {
      // Convert date to ISO format if provided
      const dueDateISO = formData.dueDate
        ? new Date(formData.dueDate).toISOString()
        : undefined;

      const submitData = {
        ...formData,
        dueDate: dueDateISO,
        description: formData.description?.trim() || undefined,
      };

      if (isEditing && task) {
        await updateTask(task.id, submitData);
      } else {
        await createTask(submitData);
      }

      onSuccess?.();
      onClose();
    } catch (error) {
      console.error('Error submitting task:', error);
      setErrors({ submit: 'Failed to save task. Please try again.' });
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    // Clear error for this field when user starts typing
    if (errors[name]) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[name];
        return newErrors;
      });
    }
  };

  return (
    <div
      className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50"
      role="dialog"
      aria-modal="true"
      aria-labelledby="task-form-title"
      onClick={(e) => {
        if (e.target === e.currentTarget) {
          onClose();
        }
      }}
    >
      <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
          <h2 id="task-form-title" className="text-xl font-bold text-gray-900">
            {isEditing ? 'Edit Task' : 'Create New Task'}
          </h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 transition-colors"
            aria-label="Close dialog"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          {errors.submit && (
            <div
              className="bg-danger-50 border border-danger-200 text-danger-700 px-4 py-3 rounded-md"
              role="alert"
            >
              {errors.submit}
            </div>
          )}

          <div>
            <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-1">
              Title <span className="text-danger-500">*</span>
            </label>
            <input
              type="text"
              id="title"
              name="title"
              value={formData.title}
              onChange={handleChange}
              className={`w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                errors.title ? 'border-danger-500' : 'border-gray-300'
              }`}
              aria-invalid={!!errors.title}
              aria-describedby={errors.title ? 'title-error' : undefined}
              required
              maxLength={200}
            />
            {errors.title && (
              <p id="title-error" className="mt-1 text-sm text-danger-600" role="alert">
                {errors.title}
              </p>
            )}
            <p className="mt-1 text-xs text-gray-500">
              {formData.title.length}/200 characters
            </p>
          </div>

          <div>
            <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
              Description
            </label>
            <textarea
              id="description"
              name="description"
              value={formData.description}
              onChange={handleChange}
              rows={4}
              className={`w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                errors.description ? 'border-danger-500' : 'border-gray-300'
              }`}
              aria-invalid={!!errors.description}
              aria-describedby={errors.description ? 'description-error' : undefined}
              maxLength={1000}
            />
            {errors.description && (
              <p id="description-error" className="mt-1 text-sm text-danger-600" role="alert">
                {errors.description}
              </p>
            )}
            <p className="mt-1 text-xs text-gray-500">
              {formData.description?.length || 0}/1000 characters
            </p>
          </div>

          <div>
            <label htmlFor="dueDate" className="block text-sm font-medium text-gray-700 mb-1">
              Due Date
            </label>
            <input
              type="date"
              id="dueDate"
              name="dueDate"
              value={formData.dueDate}
              onChange={handleChange}
              className={`w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                errors.dueDate ? 'border-danger-500' : 'border-gray-300'
              }`}
              aria-invalid={!!errors.dueDate}
              aria-describedby={errors.dueDate ? 'dueDate-error' : undefined}
              min={new Date().toISOString().split('T')[0]}
            />
            {errors.dueDate && (
              <p id="dueDate-error" className="mt-1 text-sm text-danger-600" role="alert">
                {errors.dueDate}
              </p>
            )}
          </div>

          <div className="flex flex-wrap gap-3 pt-4 border-t border-gray-200">
            <button
              type="submit"
              disabled={isSubmitting}
              className={`flex-1 px-6 py-2 bg-primary-600 text-white rounded-md font-medium hover:bg-primary-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed ${
                isSubmitting ? 'opacity-50 cursor-not-allowed' : ''
              }`}
            >
              {isSubmitting ? 'Saving...' : isEditing ? 'Update Task' : 'Create Task'}
            </button>
            <button
              type="button"
              onClick={onClose}
              className="flex-1 px-6 py-2 bg-gray-100 text-gray-700 rounded-md font-medium hover:bg-gray-200 transition-colors"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

