import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { TaskProvider } from './context/TaskContext';
import { Home } from './pages/Home';

/**
 * Main App component.
 * Sets up routing and provides the Task context to all child components.
 */
function App() {
  return (
    <TaskProvider>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="*" element={<Home />} />
      </Routes>
    </TaskProvider>
  );
}

export default App;

