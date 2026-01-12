import React, { createContext, useContext, useState, useCallback, ReactNode } from 'react';
import { NotificationType } from '../components/Notification';

/**
 * Interface for notification item.
 */
interface NotificationItem {
  id: string;
  type: NotificationType;
  message: string;
}

/**
 * Context type for notifications.
 */
interface NotificationContextType {
  notifications: NotificationItem[];
  showNotification: (type: NotificationType, message: string, duration?: number) => void;
  showSuccess: (message: string, duration?: number) => void;
  showError: (message: string, duration?: number) => void;
  showWarning: (message: string, duration?: number) => void;
  showInfo: (message: string, duration?: number) => void;
  removeNotification: (id: string) => void;
  clearNotifications: () => void;
}

const NotificationContext = createContext<NotificationContextType | undefined>(undefined);

/**
 * Props for NotificationProvider component.
 */
interface NotificationProviderProps {
  children: ReactNode;
}

/**
 * Notification context provider.
 * Manages notification state and provides methods to display notifications.
 */
export const NotificationProvider: React.FC<NotificationProviderProps> = ({ children }) => {
  const [notifications, setNotifications] = useState<NotificationItem[]>([]);

  const showNotification = useCallback(
    (type: NotificationType, message: string, duration = 5000) => {
      const id = Date.now().toString() + Math.random().toString(36).substr(2, 9);
      const newNotification: NotificationItem = { id, type, message };

      setNotifications((prev) => [...prev, newNotification]);

      // Auto-remove after duration if specified
      if (duration > 0) {
        setTimeout(() => {
          removeNotification(id);
        }, duration);
      }
    },
    []
  );

  const showSuccess = useCallback(
    (message: string, duration?: number) => {
      showNotification(NotificationType.Success, message, duration);
    },
    [showNotification]
  );

  const showError = useCallback(
    (message: string, duration = 7000) => {
      // Errors stay longer by default (7 seconds)
      showNotification(NotificationType.Error, message, duration);
    },
    [showNotification]
  );

  const showWarning = useCallback(
    (message: string, duration?: number) => {
      showNotification(NotificationType.Warning, message, duration);
    },
    [showNotification]
  );

  const showInfo = useCallback(
    (message: string, duration?: number) => {
      showNotification(NotificationType.Info, message, duration);
    },
    [showNotification]
  );

  const removeNotification = useCallback((id: string) => {
    setNotifications((prev) => prev.filter((n) => n.id !== id));
  }, []);

  const clearNotifications = useCallback(() => {
    setNotifications([]);
  }, []);

  const value: NotificationContextType = {
    notifications,
    showNotification,
    showSuccess,
    showError,
    showWarning,
    showInfo,
    removeNotification,
    clearNotifications,
  };

  return <NotificationContext.Provider value={value}>{children}</NotificationContext.Provider>;
};

/**
 * Hook to use the notification context.
 * Must be used within a NotificationProvider.
 */
export const useNotification = (): NotificationContextType => {
  const context = useContext(NotificationContext);
  if (context === undefined) {
    throw new Error('useNotification must be used within a NotificationProvider');
  }
  return context;
};

