# 📰 Blog System API

A modular, test-driven blog system built with **ASP.NET Core**, **Clean Architecture**, and **Domain-Driven Design (DDD)**.  
The system supports full CRUD operations for **Blogs** and **Authors**, uses **CQRS with MediatR**, and is fully containerized using **Docker**.

---

## 🛠️ Tech Stack

- **.NET 9 (ASP.NET Core Web API)**
- **Entity Framework Core**
- **SQL Server **
- **CQRS + MediatR**
- **Clean Architecture + DDD principles**
- **Unit of Work with Repository Pattern**
- **xUnit + FluentAssertions (TDD)**
- **Docker + Docker Compose**

---

## 📁 Project Structure (Clean Architecture)

```bash
├── BlogSystem.API           # Presentation layer (Controllers, Middleware)
├── BlogSystem.Application   # Use cases (CQRS, Services, DTOs)
├── BlogSystem.Domain        # Domain models, interfaces, value objects
├── BlogSystem.Infrastructure# Data access (EF Core, DbContext, Repos)
└── BlogSystem.Tests         # Unit tests (xUnit + FluentAssertions)

## 🚀 Getting Started (With Commands)

### 1. Clone the Repository

```bash
git clone https://github.com/<your-username>/BlogSystem.git
cd BlogSystem

### 2. Restor .NET Dependencies
dotnet restore

### 3.Configure Environment Variables
#### Create a .env file in the root folder with:
ConnectionStrings__DefaultConnection=Server=localhost;Database=BlogDb;TrustServerCertificate=True;

### 4. Add Migrations
dotnet ef migrations add InitialCreate -p BlogSystem.Infrastructure -s BlogSystem.API

### 5. Apply Migrations to Create Database
dotnet ef database update -p BlogSystem.Infrastructure -s BlogSystem.API

### 6. Run the API Locally
dotnet run --project BlogSystem.API

