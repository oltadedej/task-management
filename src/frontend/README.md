# Task Management Frontend

A modern, responsive, and accessible React TypeScript application for managing tasks. Built with the latest React 18 features, TypeScript, and Tailwind CSS.

## Features

- ✅ **Full CRUD Operations**: Create, read, update, and delete tasks
- ✅ **Status Management**: Mark tasks as complete or incomplete
- ✅ **Status Filtering**: Filter tasks by status (Not Started, In Progress, Completed)
- ✅ **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices
- ✅ **Accessible UI**: Built with accessibility in mind (ARIA labels, keyboard navigation, screen reader support)
- ✅ **Modern Tech Stack**: React 18, TypeScript, Vite, Tailwind CSS
- ✅ **State Management**: React Context API for centralized state management
- ✅ **Error Handling**: Comprehensive error handling with user-friendly popup notifications
- ✅ **Notification System**: Toast notifications for success, error, warning, and info messages
- ✅ **Form Validation**: Client-side validation with helpful error messages
- ✅ **Loading States**: Visual feedback during API operations

## Technology Stack

- **React 18.2.0** - Latest React with functional components and hooks
- **TypeScript 5.2.2** - Type-safe development
- **Vite 5.0.8** - Fast build tool and dev server
- **Tailwind CSS 3.3.6** - Utility-first CSS framework
- **React Router 6.20.0** - Client-side routing
- **Axios 1.6.2** - HTTP client for API calls
- **date-fns 2.30.0** - Date formatting utilities

## Prerequisites

Before you begin, ensure you have the following installed:

- **Node.js** (v18 or higher recommended)
- **npm** or **yarn** package manager
- **Backend API** running (see backend README for setup instructions)

## Getting Started

### 1. Install Dependencies

Navigate to the frontend directory and install the required packages:

```bash
cd src/frontend
npm install
```

or

```bash
cd src/frontend
yarn install
```

### 2. Configure Backend URL

The frontend is configured to connect to the backend API at `https://localhost:7027/api/v1` by default (HTTPS).

**Vite Proxy Configuration:**
- The `vite.config.ts` is configured with a proxy that forwards `/api` requests to `https://localhost:7027`
- This allows you to make requests without CORS issues during development

**To change the API URL:**

1. Create a `.env` file in the `src/frontend` directory:

```env
VITE_API_BASE_URL=https://localhost:7027/api/v1
```

2. Replace `https://localhost:7027` with your backend URL if it's running on a different address.

**Note**: 
- In Vite, environment variables must be prefixed with `VITE_` to be accessible in the client code.
- If your backend uses a self-signed certificate (common in development), you may need to accept the certificate in your browser or configure axios to ignore SSL errors (not recommended for production).
- The proxy in `vite.config.ts` will handle HTTPS connections to the backend during development.

### 3. Start Development Server

Run the development server:

```bash
npm run dev
```

or

```bash
yarn dev
```

The application will be available at `http://localhost:3000` (or another port if 3000 is in use).

### 4. Build for Production

To create a production build:

```bash
npm run build
```

or

```bash
yarn build
```

The production-ready files will be in the `dist` directory.

### 5. Preview Production Build

To preview the production build locally:

```bash
npm run preview
```

or

```bash
yarn preview
```

## Project Structure

```
src/frontend/
├── src/
│   ├── components/          # Reusable React components
│   │   ├── TaskCard.tsx     # Individual task card component
│   │   ├── TaskList.tsx     # List of tasks component
│   │   ├── TaskForm.tsx     # Form for creating/editing tasks
│   │   ├── FilterBar.tsx    # Filter tasks by status
│   │   └── Notification.tsx # Toast notification component
│   ├── context/             # React Context API
│   │   ├── TaskContext.tsx  # Global task state management
│   │   └── NotificationContext.tsx # Global notification state management
│   ├── pages/               # Page components
│   │   └── Home.tsx         # Main home page
│   ├── services/            # API services
│   │   └── api.service.ts   # Axios-based API client with error handling
│   ├── types/               # TypeScript type definitions
│   │   └── task.types.ts    # Task-related types and interfaces
│   ├── App.tsx              # Main App component with providers
│   ├── main.tsx             # Application entry point
│   └── index.css            # Global styles and Tailwind imports
├── public/                  # Static assets
├── index.html               # HTML template
├── package.json             # Dependencies and scripts
├── tsconfig.json            # TypeScript configuration
├── vite.config.ts           # Vite configuration
├── tailwind.config.js       # Tailwind CSS configuration
└── README.md                # This file
```

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint to check code quality

## API Integration

The frontend communicates with the backend API using the following endpoints:

### Base URL Configuration

The API base URL is configured in `src/services/api.service.ts` and defaults to `https://localhost:7027/api/v1` (HTTPS).

**Configuration Options:**
1. **Environment Variable**: Set `VITE_API_BASE_URL` in `.env` file
2. **Vite Proxy**: Configured in `vite.config.ts` to proxy `/api` requests to `https://localhost:7027`

**Error Handling:**
- All API errors are intercepted and converted to user-friendly messages
- HTTP status codes are handled appropriately:
  - **400**: Invalid request - validation errors
  - **404**: Resource not found - task not found, endpoint doesn't exist
  - **500**: Server error - internal server error
  - **503**: Service unavailable - server temporarily unavailable
  - **Network Errors**: Connection issues, server unreachable
- Error notifications appear as popups in the top-right corner
- Success notifications appear for successful operations

### API Endpoints

| Method | Endpoint | Description | Error Handling |
|--------|----------|-------------|----------------|
| GET | `/api/v1/tasks` | Get all tasks | Shows error notification on failure |
| GET | `/api/v1/tasks/{id}` | Get task by ID | Shows "Task not found" notification for 404 |
| GET | `/api/v1/tasks/status/{status}` | Get tasks by status (0=NotStarted, 1=InProgress, 2=Completed) | Shows error notification on failure |
| POST | `/api/v1/tasks` | Create a new task | Shows success notification on success, error notification on failure |
| PUT | `/api/v1/tasks/{id}` | Update an existing task | Shows "Task not found" for 404, validation errors for 400, success notification on success |
| DELETE | `/api/v1/tasks/{id}` | Delete a task | Shows "Task not found" for 404, success notification on success |
| PATCH | `/api/v1/tasks/{id}/complete` | Mark task as complete | Shows "Task not found" for 404, success notification on success |
| PATCH | `/api/v1/tasks/{id}/incomplete` | Mark task as incomplete (Not Started) | Shows "Task not found" for 404, success notification on success |

### Request/Response Formats

#### Create Task Request

```typescript
{
  title: string;           // Required, max 200 characters
  description?: string;    // Optional, max 1000 characters
  dueDate?: string;        // Optional, ISO 8601 date string
}
```

#### Update Task Request

```typescript
{
  title: string;           // Required, max 200 characters
  description?: string;    // Optional, max 1000 characters
  dueDate?: string;        // Optional, ISO 8601 date string
}
```

#### Task Response

```typescript
{
  id: string;              // GUID
  title: string;
  description?: string;
  status: number;          // 0=NotStarted, 1=InProgress, 2=Completed
  createdAt: string;       // ISO 8601 datetime
  updatedAt: string;       // ISO 8601 datetime
  dueDate?: string;        // ISO 8601 datetime
}
```

## Features Overview

### Task Management

- **Create Tasks**: Click the "New Task" button to create a new task with title, description, and optional due date
- **Edit Tasks**: Click the "Edit" button on any task card to modify task details
- **Delete Tasks**: Click the "Delete" button to permanently remove a task (with confirmation)
- **Mark Complete/Incomplete**: Toggle task status between complete and incomplete

### Filtering

- **All Tasks**: View all tasks regardless of status
- **Not Started**: Filter tasks that haven't been started (status = 0)
- **In Progress**: Filter tasks currently in progress (status = 1)
- **Completed**: Filter completed tasks (status = 2)

### User Experience

- **Responsive Design**: The UI adapts to different screen sizes
- **Loading States**: Visual feedback during API operations
- **Error Handling**: User-friendly popup notifications for all errors including 404, 500, and network errors
- **Success Notifications**: Toast notifications for successful operations (create, update, delete, status changes)
- **Form Validation**: Real-time validation with helpful error messages
- **Accessibility**: Full keyboard navigation and screen reader support
- **Notification System**: 
  - **Error Notifications**: Displayed for API errors (404, 500, network issues, etc.) with user-friendly messages
  - **Success Notifications**: Displayed when tasks are created, updated, deleted, or status is changed
  - **Auto-dismiss**: Notifications automatically disappear after a few seconds (configurable)
  - **Manual Dismiss**: Users can close notifications by clicking the X button
  - **Accessible**: ARIA labels and roles for screen reader support

## Notification System

The application includes a comprehensive notification system for user feedback:

### Features

- **Error Notifications**: Displayed when API calls fail
  - 404 errors: "The requested resource was not found"
  - 500 errors: "Server error. Please try again later"
  - Network errors: "Unable to connect to the server. Please check your internet connection"
  - Validation errors: Shows validation error messages from the backend
  - Custom error messages for all error types

- **Success Notifications**: Displayed for successful operations
  - Task created: "Task created successfully!"
  - Task updated: "Task updated successfully!"
  - Task deleted: "Task deleted successfully!"
  - Status changed: "Task marked as complete/incomplete!"

- **Notification Behavior**:
  - Auto-dismiss after 5 seconds (errors stay for 7 seconds)
  - Manual dismiss by clicking the X button
  - Multiple notifications stack vertically
  - Smooth slide-in animation
  - Accessible with ARIA labels and roles

### Usage

Notifications are automatically triggered by the `TaskContext` when operations succeed or fail. The notification system is integrated into all CRUD operations and provides consistent user feedback across the application.

## Accessibility Features

- Semantic HTML elements (`<header>`, `<main>`, `<article>`, etc.)
- ARIA labels and roles for screen readers
- Keyboard navigation support
- Focus indicators for interactive elements
- Color contrast compliant with WCAG standards
- Form validation with descriptive error messages
- Accessible notifications with proper ARIA attributes

## Browser Support

The application supports modern browsers:

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Troubleshooting

### Backend Connection Issues

If you're experiencing connection issues with the backend:

1. **Ensure the backend API is running and accessible**
   - Backend should be running on `https://localhost:7027` (HTTPS)
   - Check that Swagger UI is accessible at `https://localhost:7027/swagger`

2. **Check HTTPS Certificate Issues**
   - If using a self-signed certificate, your browser may block the connection
   - Accept the certificate in your browser when prompted
   - For development, you may see SSL warnings - accept them to proceed

3. **Verify Configuration**
   - Check the `VITE_API_BASE_URL` environment variable matches your backend URL
   - Verify the proxy configuration in `vite.config.ts` is correct
   - Default: `https://localhost:7027/api/v1`

4. **CORS Configuration**
   - CORS is already configured in the backend to allow all origins
   - If issues persist, check the backend CORS configuration (see backend README)

5. **Network Errors**
   - Check browser console for detailed error messages
   - Error notifications will appear in the top-right corner with user-friendly messages
   - Common errors:
     - **404 Not Found**: "The requested resource was not found"
     - **500 Server Error**: "Server error. Please try again later"
     - **Network Error**: "Unable to connect to the server. Please check your internet connection"

### Build Errors

If you encounter build errors:

1. Clear node_modules and reinstall: `rm -rf node_modules && npm install`
2. Clear Vite cache: `rm -rf node_modules/.vite`
3. Ensure you're using the correct Node.js version (v18 or higher)

### Port Already in Use

If port 3000 is already in use:

- Vite will automatically try the next available port
- Check the terminal output for the actual port number
- Or specify a different port in `vite.config.ts`:

```typescript
server: {
  port: 3001, // Change to your preferred port
}
```

## Development Guidelines

### Code Style

- Follow TypeScript best practices
- Use functional components with hooks
- Keep components small and focused
- Use meaningful variable and function names
- Add comments for complex logic

### Component Structure

```typescript
// 1. Imports
import React from 'react';
import { SomeType } from '../types';

// 2. Interfaces/Types
interface ComponentProps {
  // props definition
}

// 3. Component
export const Component: React.FC<ComponentProps> = ({ prop1, prop2 }) => {
  // Component logic
  return (
    // JSX
  );
};
```

### State Management

- Use React Context API for global state (tasks, loading, errors)
- Use local state (useState) for component-specific state
- Use useEffect for side effects and data fetching

### API Calls

- All API calls should go through the `apiService` in `src/services/api.service.ts`
- Use the `TaskContext` for managing task-related state
- Always handle errors appropriately and provide user feedback

## Contributing

1. Follow the existing code style
2. Ensure TypeScript types are properly defined
3. Test your changes thoroughly
4. Ensure accessibility standards are maintained
5. Update documentation as needed

## License

This project is part of the Task Management System trial project.

## HTTPS Configuration

The frontend is configured to communicate with the backend over HTTPS (`https://localhost:7027`).

### Development Setup

**Important Notes for HTTPS Development:**

1. **Self-Signed Certificates**: 
   - The backend may use a self-signed certificate for HTTPS in development
   - Your browser may show security warnings - you can safely accept them for local development
   - The Vite proxy is configured to handle HTTPS connections

2. **Certificate Acceptance**:
   - When first accessing `https://localhost:7027`, your browser may prompt you to accept the certificate
   - Click "Advanced" → "Proceed to localhost" (or similar option)
   - This is safe for local development

3. **Proxy Configuration**:
   - The Vite dev server proxies `/api` requests to `https://localhost:7027`
   - This avoids CORS issues and handles HTTPS connections automatically
   - See `vite.config.ts` for proxy configuration

4. **Production Considerations**:
   - In production, use valid SSL certificates
   - Update `VITE_API_BASE_URL` environment variable to your production API URL
   - Ensure CORS is properly configured on the backend

## Support

For issues or questions:

1. Check the troubleshooting section above
2. Review the backend API documentation (Swagger UI at `https://localhost:7027/swagger`)
3. Check browser console for detailed error messages
4. Verify backend API is running and accessible at `https://localhost:7027`
5. Check for SSL certificate issues (accept self-signed certificates in development)
6. Error notifications will appear in the top-right corner with helpful messages

---

**Last Updated**: January 2026

