# PizzaStore REST API (.NET 8 + Clean Architecture)

A production-structured RESTful backend API built with **ASP.NET Core 8 Minimal APIs**, **Entity Framework Core**, **JWT Bearer Authentication**, **xUnit Integration Testing**, and **Docker Compose**.

## Architecture

Structured into 4 Clean Architecture layers enforcing inward-only dependencies:

- **`PizzaStore.Domain`**: Core entities (`Pizza`, `Topping`, `ApplicationUser`) with zero external dependencies.
- **`PizzaStore.Application`**: Request/Response DTOs, DataAnnotations validation contracts, and service interfaces (`IPizzaService`, `IAuthService`).
- **`PizzaStore.Infrastructure`**: EF Core `PizzaDb` context, SQL Server migrations, `PasswordHasher`, JWT token generation, and service implementations.
- **`PizzaStore` (API)**: Slim `Program.cs`, modular Minimal API endpoint groups (`AuthEndpoints`, `PizzaEndpoints`), endpoint `ValidationFilter`, and global `ProblemDetails` exception handling.
- **`PizzaStore.Tests`**: Automated integration test suite using `WebApplicationFactory` and EF Core In-Memory database.

## Tech Stack

- **.NET 8** (C# 12, Minimal APIs)
- **Entity Framework Core 8** & **SQL Server 2022**
- **JWT Bearer Authentication & Role-Based Authorization** (`Admin` / `Customer`)
- **xUnit** & `Microsoft.AspNetCore.Mvc.Testing`
- **Docker** (Multi-stage Linux runtime build) & **Docker Compose**
- **GitHub Actions** (Automated CI build & integration test pipeline)

## Quick Start with Docker

1. Clone the repository:
   ```bash
   git clone [https://github.com/azaad-test/PizzaStore.git](https://github.com/azaad-test/PizzaStore.git)
   cd PizzaStore