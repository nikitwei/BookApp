# BookApp API

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15.1-336791?logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?logo=docker)
![Architecture](https://img.shields.io/badge/Architecture-Clean-brightgreen)

BookApp is a robust, secure, and scalable backend RESTful API built for reading book applications. It is engineered with C# and .NET 10, strictly adhering to **Clean Architecture** principles. The system orchestrates comprehensive user management, book catalogs, and personalized reading list activities.

## 🏗 Architecture & Tech Stack

This project follows **Clean Architecture** to ensure strict separation of concerns, high testability, and long-term maintainability.

* **Language**: C# 12
* **Framework**: .NET 10.0
* **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API)
* **Database**: PostgreSQL
* **ORM**: Entity Framework Core (EF Core)
* **Authentication**: JSON Web Tokens (JWT) & BCrypt
* **Testing**: xUnit, Moq, InMemory EF Core
* **Containerization**: Docker & Docker Compose

### Project Structure
```plaintext
BookApp.Solution/
├── BookApp.Domain          # Core business entities, enums, and repository interfaces
├── BookApp.Application     # Application logic, DTOs, Application Services, Interfaces
├── BookApp.Infrastructure  # EF Core DbContext, Data Access, JWT Generation
├── BookApp.Api             # RESTful API Controllers, Middleware, Swagger, DI Setup
└── BookApp.Tests           # Comprehensive Unit Tests covering 100% of edge cases
```

## 🚀 Features

* **Authentication & Authorization**: Secure JWT-based authentication with BCrypt password hashing. Role-based access control (Admin vs User).
* **Book Management Catalog**: Full CRUD operations for Books, featuring pagination and complex search filtering.
* **Personalized Reading Lists**: Users can seamlessly manage their own reading lists, update statuses (e.g., *Want to Read*, *Reading*, *Completed*), and rate books.
* **Global Error Handling**: Standardized exception handling returning unified JSON responses.
* **100% Test Coverage**: In-depth unit tests across Service and Controller layers handling positive paths and complex edge cases.

## 🛠 Getting Started

### Prerequisites
* [Docker Desktop](https://www.docker.com/products/docker-desktop)
* [.NET 10 SDK](https://dotnet.microsoft.com/download) (For local non-Docker development)

### Running with Docker (Recommended)

The easiest way to get the entire ecosystem (API + PostgreSQL) up and running is via Docker Compose.

1. Clone the repository and navigate to the project root.
2. Rename the provided `.env_example` file to `.env`:
   ```bash
   mv .env_example .env
   ```
   *(Update the values inside `.env` if necessary)*
3. Spin up the containers:
   ```bash
   docker-compose up --build -d
   ```
3. The API will be available at `http://localhost:8080`.
4. Access the Swagger UI for interactive API documentation at: `http://localhost:8080/swagger`

### Running Locally (Without Docker)

1. Rename `.env_example` to `.env` in the root folder.
2. Ensure you have a running PostgreSQL instance and update the connection string inside your `.env` file.
3. Apply EF Core Migrations:
   ```bash
   cd BookApp.Solution/BookApp.Api
   dotnet ef database update
   ```
3. Run the application:
   ```bash
   dotnet run
   ```

## 🧪 Testing

The solution is heavily tested using `xUnit` and `Moq`. The test suite utilizes an In-Memory Database to perfectly isolate service layer logic.

To run the test suite:
```bash
dotnet test BookApp.Solution/BookApp.Tests/BookApp.Tests.csproj
```
*Current Coverage: 40/40 Tests Passing (Service Layer & Controller Layer)*

## 📚 API Endpoints Overview

For a detailed, interactive map of all endpoints, please run the application and visit the `/swagger` endpoint.

### Authentication
* `POST /api/auth/register` - Register a new user
* `POST /api/auth/login` - Authenticate and retrieve JWT

### Books
* `GET /api/books` - Retrieve paginated and filtered catalog
* `GET /api/books/{id}` - Retrieve specific book details
* `POST /api/books` - Create a book (Admin)
* `PUT /api/books/{id}` - Update a book (Admin)
* `DELETE /api/books/{id}` - Remove a book (Admin)

### Reading List (Requires JWT)
* `GET /api/reading-list` - Fetch user's reading list
* `POST /api/reading-list` - Add book to list
* `PUT /api/reading-list/{id}` - Update status / rating
* `DELETE /api/reading-list/{id}` - Remove book from list

## 🔒 Security Practices
* **Hashing**: Zero-knowledge password storage via `BCrypt.Net-Next`.
* **JWT Lifespans**: Tokens are issued securely using `HS256` symmetric algorithms.
* **SQL Injection**: Prevented intrinsically via Entity Framework Core parameterized querying.
