# OutlayService - Employee Management API

A professional ASP.NET Core Web API for managing employee records with full CRUD operations built with .NET 10.

## Features

- **Complete CRUD Operations**: Create, Read, Update, and Delete employee records
- **Entity Framework Core**: Data access using EF Core with SQL Server
- **RESTful API**: Well-structured REST endpoints
- **Data Validation**: Input validation with data annotations
- **Error Handling**: Comprehensive error handling and logging
- **Async Operations**: Fully asynchronous operations for better performance
- **Generic Response Wrapper**: Standardized API responses with success/failure status

## Project Structure

```
OutlayService/
??? Models/
?   ??? Employee.cs                    # Employee entity model
??? DTOs/
?   ??? EmployeeDto.cs                 # Response DTO
?   ??? CreateEmployeeDto.cs           # Create request DTO
?   ??? UpdateEmployeeDto.cs           # Update request DTO
??? Services/
?   ??? Interfaces/
?   ?   ??? IEmployeeService.cs        # Service interface
?   ??? Impl/
?       ??? EmployeeService.cs         # Service implementation
??? Controllers/
?   ??? EmployeesController.cs         # API controller
??? Data/
?   ??? ApplicationDbContext.cs        # EF Core DbContext
??? Responses/
?   ??? OutlayServiceResponse.cs       # Generic response wrapper
??? Program.cs                         # Application configuration
```

## Database Setup

### Prerequisites
- SQL Server (Local or Remote)
- .NET 10 SDK

### Connection String
Update `appsettings.json` with your SQL Server connection string:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=OutlayServiceDb;Trusted_Connection=true;Encrypt=false;"
}
```

### Create Database
Run these commands to create the database:

```bash
dotnet ef database update
```

Or create migrations manually:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Entity Model

### Employee
```csharp
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}
```

**Constraints:**
- Name: Required, Max 100 characters
- Email: Required, Max 256 characters, Unique
- CreatedOn: Auto-generated (UTC)
- UpdatedOn: Auto-generated (UTC)

## API Endpoints

### Base URL
```
https://localhost:7001/api/employees
```

### Get All Employees
```http
GET /api/employees
```

**Response:**
```json
{
  "success": true,
  "message": "Retrieved X employees successfully",
  "result": [
    {
      "id": 1,
      "name": "John Doe",
      "email": "john@example.com",
      "createdOn": "2024-01-01T10:00:00Z",
      "updatedOn": "2024-01-01T10:00:00Z"
    }
  ]
}
```

### Get Employee by ID
```http
GET /api/employees/{id}
```

### Create Employee
```http
POST /api/employees
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com"
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "Employee created successfully",
  "result": {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "createdOn": "2024-01-01T10:00:00Z",
    "updatedOn": "2024-01-01T10:00:00Z"
  }
}
```

### Update Employee
```http
PUT /api/employees/{id}
Content-Type: application/json

{
  "name": "Jane Doe",
  "email": "jane@example.com"
}
```

### Delete Employee
```http
DELETE /api/employees/{id}
```

**Response:**
```json
{
  "success": true,
  "message": "Employee deleted successfully",
  "result": true
}
```

## Response Format

All API responses follow a standardized format:

```json
{
  "success": true/false,
  "message": "Response message",
  "result": null/object
}
```

## Error Handling

- **400 Bad Request**: Invalid input data
- **404 Not Found**: Employee not found
- **409 Conflict**: Email already exists
- **500 Internal Server Error**: Server error

## Validation

### Create/Update Employee
- **Name**: Required, 2-100 characters
- **Email**: Required, valid email format, unique

## Running the Application

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

The API will be available at `https://localhost:7001`

## Best Practices Implemented

- ? **Separation of Concerns**: Controllers, Services, DTOs properly separated
- ? **Async/Await**: All database operations are asynchronous
- ? **Validation**: Input validation with data annotations
- ? **Error Handling**: Try-catch blocks with proper logging
- ? **Logging**: Integrated logging for debugging
- ? **DTOs**: Request/Response DTOs for data transfer
- ? **Generic Response Wrapper**: Standardized API responses
- ? **EF Core Best Practices**: Proper DbContext configuration, indexes
- ? **No Tracking Queries**: AsNoTracking() for read-only operations
- ? **Unique Constraints**: Email uniqueness enforced

## Technology Stack

- **.NET 10**
- **ASP.NET Core Web API**
- **Entity Framework Core 10.0.0**
- **SQL Server**
- **C# 13**

## License

This project is part of the OutlayService ecosystem.
