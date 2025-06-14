PlayStation Lounge Management System - Backend API

Project Description: A robust RESTful API for a PlayStation lounge management system, handling room bookings, menu orders, and administrative functions with secure authentication and efficient data management.

Technical Stack:

Framework: ASP.NET Core 7.0
Database: SQL Server with Entity Framework Core
Authentication: JWT Bearer Tokens
Architecture: Clean Architecture
Tools: Swagger/OpenAPI, Git
Key Features:

RESTful API endpoints for all system operations
Secure authentication and role-based authorization
Entity Framework Core for database operations
Repository pattern implementation
DTOs for data transfer
Global error handling
API versioning
CORS policy configuration
Technical Implementation:

Built with ASP.NET Core Web API
Entity Framework Core Code-First approach
JWT for secure authentication
AutoMapper for object mapping
FluentValidation for request validation
Dependency Injection
Asynchronous programming
LINQ for data querying
SQL Server for data persistence
Example Project Structure:

CopyInsert
PlayStationLounge.API/
├── Controllers/
├── Models/
├── DTOs/
├── Interfaces/
├── Repositories/
├── Services/
├── Mappings/
└── Middleware/
Endpoints Examples:

POST /api/auth/register - User registration
POST /api/auth/login - User login
GET /api/rooms/available - Get available rooms
POST /api/bookings - Create new booking
GET /api/menu/items - Get menu items
POST /api/orders - Place new order
Database Schema:

Users
Roles
Rooms
Bookings
MenuItems
Categories
Orders
OrderItems
How to Run:

Clone the repository
Update connection string in appsettings.json
Run dotnet ef database update
Run the application
Dependencies:

Microsoft.EntityFrameworkCore
Microsoft.AspNetCore.Authentication.JwtBearer
AutoMapper
Swashbuckle.AspNetCore
Microsoft.EntityFrameworkCore.SqlServer
