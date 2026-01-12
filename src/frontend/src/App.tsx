import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { NotificationProvider, useNotification } from './context/NotificationContext';
import { NotificationContainer } from './components/Notification';
import { TaskProvider } from './context/TaskContext';
import { Home } from './pages/Home';

/**
 * Inner app component that uses notifications.
 * This is separated because useNotification must be within NotificationProvider.
 */
const AppContent: React.FC = () => {
  const { notifications, removeNotification } = useNotification();

  return (
    <>
      <TaskProvider>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="*" element={<Home />} />
        </Routes>
      </TaskProvider>
      <NotificationContainer
        notifications={notifications}
        onClose={removeNotification}
      />
    </>
  );
};

/**
 * Main App component.
 * Sets up routing and provides contexts to all child components.
 */
function App() {
  return (
    <NotificationProvider>
      <AppContent />
    </NotificationProvider>
  );
}

export default App;

