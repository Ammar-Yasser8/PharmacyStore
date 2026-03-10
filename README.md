# PharmacyStore API 🏥

A robust RESTful API built with **ASP.NET Core** and **Entity Framework Core**. This project is structured using **Clean Architecture** principles to ensure separation of concerns, scalability, and maintainability.

## 🏗️ Architecture Layers

The solution is divided into the following layers:

1. **Pharmacy.API**: The presentation layer containing the ASP.NET Core Web API controllers, Swagger for documentation, and Dependency Injection setups.
2. **Pharmacy.Domain**: The enterprise logic layer containing the core domain entities (`Category`, `Product`), and the contract interfaces (`Repositories.Contract`, `Specification`).
3. **Pharmacy.Repository**: The infrastructure and data access layer. It implements the Repository Pattern (`GenericRepository`) and Specification Pattern (`SpecificationEvaluator`), along with the `PharmacyDBContext`.
4. **Pharmacy.Services**: The business logic and orchestration layer.

## 📂 Domain Entities

### Category
Supports localization for Arabic, English, and Russian.
- `Id`
- `NameAr` 
- `NameEn` 
- `NameRu`
- `ImageUrl`
- `Products` (One-to-Many relationship)

### Product
- `Id`
- `Name`
- `Description`
- `Price`
- `Stock`
- `ImageUrl`
- `CategoryId`

## 🛠️ Technologies Used
- **.NET 8** (or current .NET Core version)
- **ASP.NET Core Web API**
- **Entity Framework Core** (SQL Server)
- **Repository Pattern**
- **Specification Pattern**
- **Swagger / OpenAPI**

## 🚀 Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download)
- SQL Server (Update the `DefaultConnection` string in `appsettings.json`)

### Setup Instructions
1. Clone the repository:
   ```bash
   git clone https://github.com/Ammar-Yasser8/PharmacyStore.git
   ```
2. Navigate to the API directory:
   ```bash
   cd PharmacyStore/Pharmacy.API
   ```
3. Update the database connection string in `Pharmacy.API/appsettings.json`.
4. Run Entity Framework migrations to update the database:
   ```bash
   dotnet ef database update --project ../Pharmacy.Repository --startup-project .
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

### API Documentation
Once the application is running in the Development environment, you can access the Swagger UI to interact with the API endpoints at:
`https://localhost:<port>/swagger`
