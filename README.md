# Task Management System

A full-stack task management application built with modern technologies, providing a comprehensive solution for creating, organizing, and tracking tasks.

## Overview

This project is a complete task management system consisting of:

- **Backend API**: A RESTful API built with .NET 8, implementing Clean Architecture, CQRS pattern with MediatR, and Entity Framework Core
- **Frontend Application**: A modern React TypeScript application with a responsive UI built using Tailwind CSS

## Project Structure

```
src/
├── backend/          # .NET 8 Backend API
│   └── TaskManagement/
└── frontend/         # React TypeScript Frontend Application
```

## Features

- ✅ **Full CRUD Operations**: Create, read, update, and delete tasks
- ✅ **Task Status Management**: Mark tasks as Not Started, In Progress, or Completed
- ✅ **Status Filtering**: Filter tasks by status
- ✅ **Task Details**: Title, description, and due date support
- ✅ **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices
- ✅ **Modern Architecture**: Clean Architecture with CQRS pattern
- ✅ **Comprehensive Testing**: Unit and integration tests for backend
- ✅ **API Documentation**: Swagger/OpenAPI documentation

## Getting Started

### Backend

To run the backend API locally, please refer to the [Backend README](./src/backend/TaskManagement/README.md) for detailed instructions.

**Quick Start:**
1. Navigate to `src/backend/TaskManagement`
2. Restore dependencies: `dotnet restore`
3. Build the solution: `dotnet build`
4. Run the API: `dotnet run --project TaskManagement.Api`

The API will be available at `https://localhost:7027` with Swagger documentation at `https://localhost:7027/swagger`

### Frontend

To run the frontend application locally, please refer to the [Frontend README](./src/frontend/README.md) for detailed instructions.

**Quick Start:**
1. Navigate to `src/frontend`
2. Install dependencies: `npm install`
3. Start the development server: `npm run dev`

The frontend will be available at `http://localhost:3000` (or another port if 3000 is in use)

## Technology Stack

### Backend
- **.NET 8** - Latest .NET framework
- **Entity Framework Core** - ORM for database operations
- **SQLite** - Database (can be configured for other databases)
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **Swagger/OpenAPI** - API documentation

### Frontend
- **React 18** - UI library
- **TypeScript** - Type-safe development
- **Vite** - Build tool and dev server
- **Tailwind CSS** - Utility-first CSS framework
- **Axios** - HTTP client
- **React Router** - Client-side routing

## Development

### Prerequisites

- **Backend**: .NET 8 SDK
- **Frontend**: Node.js 18.x or higher, npm or yarn

### Running Both Services

1. **Start the Backend**:
   ```bash
   cd src/backend/TaskManagement
   dotnet run --project TaskManagement.Api
   ```

2. **Start the Frontend** (in a new terminal):
   ```bash
   cd src/frontend
   npm run dev
   ```

3. Open your browser and navigate to `http://localhost:3000`

## Testing

### Backend Tests

Run all backend tests:
```bash
cd src/backend/TaskManagement
dotnet test
```

Run unit tests only:
```bash
dotnet test TaskManagement.Tests.Unit
```

Run integration tests only:
```bash
dotnet test TaskManagement.Tests.Integration
```

## API Endpoints

The backend provides the following main endpoints:

- `GET /api/v1/tasks` - Get all tasks
- `GET /api/v1/tasks/{id}` - Get task by ID
- `GET /api/v1/tasks/status/{status}` - Get tasks by status
- `POST /api/v1/tasks` - Create a new task
- `PUT /api/v1/tasks/{id}` - Update a task
- `DELETE /api/v1/tasks/{id}` - Delete a task
- `PATCH /api/v1/tasks/{id}/complete` - Mark task as complete
- `PATCH /api/v1/tasks/{id}/incomplete` - Mark task as incomplete
- `PATCH /api/v1/tasks/{id}/inprogress` - Mark task as in progress

For complete API documentation, visit the Swagger UI when the backend is running.

## Task Status Values

- `0` - NotStarted
- `1` - InProgress
- `2` - Completed

## Contributing

When contributing to this project:

1. Follow the existing code structure and patterns
2. Write unit and integration tests for new features
3. Update documentation as needed
4. Ensure all tests pass before submitting changes

## Additional Resources

- [Backend Documentation](./src/backend/TaskManagement/README.md) - Detailed backend setup and API documentation
- [Frontend Documentation](./src/frontend/README.md) - Detailed frontend setup and development guide

## License

This project is part of a task management system demonstration.

