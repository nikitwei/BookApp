# Product Requirements Document (PRD): Reading Book Application Backend

## 1. Product Overview
Product Name: BookNook API (Placeholder Name)
Platform: Backend RESTful API
Primary Focus: To provide a robust, secure, and scalable backend infrastructure for a reading book application, managing users, book catalogs, and reading activities.

## 2. Technology Stack & Architecture
* Language: C# 
* Framework: .NET (latest LTS, e.g., .NET 8)
* Architecture Pattern: CleanArchitecture (Domain, Application, Infrastructure, Presentation/API layers)
* Database: PostgreSQL
* ORM: Entity Framework Core (EF Core)
* Authentication & Authorization: JWT (JSON Web Tokens)
* API Documentation: Swagger / OpenAPI

### 3. System Architecture: Clean Architecture Directory Structure
To ensure separation of concerns, the solution will be divided into four main projects:

```plaintext
BookApp.Solution
├── BookApp.Domain          (Core entities: User, Book, ReadingList, Enums, Interfaces)
├── BookApp.Application     (Business logic: CQRS/Services, DTOs, Validators, Interfaces)
├── BookApp.Infrastructure  (Data access: EF Core DbContext, Repositories, JWT Generation)
└── BookApp.Api             (Presentation: Controllers, Program.cs, Swagger Configuration, Middlewares)
```

## 4. Functional Requirements

### 4.1 Authentication & Authorization
* Registration: Users can create an account using an email and password.
* Login: Users receive a JWT upon successful authentication.
* Security: Passwords must be hashed (e.g., using BCrypt). API endpoints must be protected using [Authorize] attributes.

### 4.2 Book Management (CRUD)
* Create: Admin/Authorized users can add new books to the catalog.
* Read: Users can fetch a list of books (with pagination and filtering by genre/author) or retrieve a specific book's details.
* Update: Admin can update book details (title, description, cover image URL, etc.).
* Delete: Admin can soft-delete or permanently delete books.

### 4.3 User Reading List (CRUD)
* Create: Users can add a book to their personal reading list.
* Read: Users can view their reading list.
* Update: Users can update the reading status (e.g., 'Want to Read', 'Currently Reading', 'Read') and personal rating.
* Delete: Users can remove a book from their reading list.

## 5. Entity Relationship Diagram (ERD)
Below is the ASCII representation of the database schema for the application:

Plaintext
+-------------------+       +-------------------+       +-------------------+
|       User        |       |    UserBook       |       |       Book        |
+-------------------+       +-------------------+       +-------------------+
| PK: Id (UUID)     |       | PK: Id (UUID)     |       | PK: Id (UUID)     |
| Email (Varchar)   | 1   * | FK: UserId        | *   1 | Title (Varchar)   |
| PasswordHash      |-------| FK: BookId        |-------| Author (Varchar)  |
| FullName (Varchar)|       | Status (Enum)     |       | Description (Text)|
| Role (Varchar)    |       | Rating (Int)      |       | ISBN (Varchar)    |
| CreatedAt (Date)  |       | AddedAt (Date)    |       | PublishedDate     |
+-------------------+       +-------------------+       +-------------------+

* Status Enum: 1 = WantToRead, 2 = Reading, 3 = Completed, 4 = Dropped

## 6. Authentication Flowchart
Here is the ASCII Flowchart depicting the JWT Login and API Access flow:

```plaintext
[ Client/Frontend ]                             [ Backend API (.NET C#) ]                    [ PostgreSQL ]
        |                                                 |                                         |
        |---- 1. POST /api/auth/login {email, pass} ----->|                                         |
        |                                                 |---- 2. Query User by Email ------------>|
        |                                                 |<--- 3. Return User Data & Hash ---------|
        |                                                 |                                         |
        |                                                 |-- 4. Verify Password Hash               |
        |                                                 |-- 5. Generate JWT                       |
        |<--- 6. Return 200 OK + JWT ---------------------|                                         |
        |                                                 |                                         |
        |---- 7. GET /api/books (Header: Bearer JWT) ---->|                                         |
        |                                                 |-- 8. Validate JWT Middleware            |
        |                                                 |---- 9. Fetch Books from DB ------------>|
        |                                                 |<--- 10. Return Book List ---------------|
        |<--- 11. Return 200 OK (JSON Data) --------------|                                         |
```

## 7. API Endpoint Specifications (Swagger Outline)
The API documentation will be generated automatically using SwaggerUI. Below are the planned routes:

### Authentication (/api/auth)
* POST /register : Create a new user account.
* POST /login : Authenticate user and return JWT.

### Books (/api/books) - Role-based access applied
* GET / : Get all books (Public or Authenticated). Includes query params for page, pageSize, search.
* GET /{id} : Get specific book details.
* POST / : Create a new book (Admin only).
* PUT /{id} : Update a book (Admin only).
* DELETE /{id} : Delete a book (Admin only).

### User's Reading List (/api/reading-list) - Requires JWT
* GET / : Get the authenticated user's reading list.
* POST / : Add a book to the user's reading list. payload: { bookId, status }
* PUT /{id} : Update reading status or rating for a specific list item.
* DELETE /{id} : Remove a book from the reading list.

## 8. Non-Functional Requirements
### Security:
* Passwords stored securely using robust hashing algorithms.
* CORS configured strictly for allowed frontend domains.
* SQL Injection prevention natively handled by EF Core.
### Scalability
* Asynchronous programming (async/await) applied across all controllers and database queries to ensure high throughput.
### Error Handling
* Implementation of a Global Exception Handling Middleware to format all errors into a standardized JSON response (e.g., RFC 7807 Problem Details).