# PharmacyStore API 🏥

A robust RESTful API built with **ASP.NET Core** and **Entity Framework Core**. This project is structured using **Clean Architecture** principles to ensure separation of concerns, scalability, and maintainability.

## 🏗️ Architecture Layers

The solution is divided into the following layers:

1. **[Pharmacy.API](./Pharmacy.API/)**: The presentation layer containing the ASP.NET Core Web API controllers, Swagger for documentation, and Dependency Injection setups.
2. **[Pharmacy.Domain](./Pharmacy.Domain/)**: The enterprise logic layer containing the core domain entities (`Category`, `Product`), and the contract interfaces (`Repositories.Contract`, `Specification`).
3. **[Pharmacy.Repository](./Pharmacy.Repository/)**: The infrastructure and data access layer. It implements the Repository Pattern (`GenericRepository`) and Specification Pattern (`SpecificationEvaluator`), along with the `PharmacyDBContext`.
4. **[Pharmacy.Services](./Pharmacy.Services/)**: The business logic and orchestration layer. Contains the generic `IImageService` for file uploads to `wwwroot/images/`.

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

### Cart
- `Id` (GUID)
- `Items` (One-to-Many relationship with `CartItem`)

### CartItem
- `Id`
- `CartId`
- `ProductId`
- `Quantity`

### AppUser (IdentityUser)
- `Id`
- `DisplayName`
- `Email`
- `UserName`
- `PhoneNumber`
- `Address` (One-to-One relationship)

### Address
- `Id`
- `Street`
- `City`
- `Country`
- `AppUserId`

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

#### 🛒 Product Endpoints

| Method | Endpoint | Description | Content-Type |
|--------|----------|-------------|--------------|
| `GET` | `/api/Product` | Get paginated products | — |
| `GET` | `/api/Product/{id}` | Get product by ID | — |
| `POST` | `/api/Product` | Create a new product | `multipart/form-data` |
| `PUT` | `/api/Product/{id}` | Update a product | `multipart/form-data` |
| `DELETE` | `/api/Product/{id}` | Delete a product | — |

**GET `/api/Product` Query Parameters:**
- `pageIndex` (int, default: 1) — Page number
- `pageSize` (int, default: 5, max: 50) — Items per page
- `categoryId` (int?) — Filter by category ID
- `search` (string?) — Search by product name (case-insensitive)
- `sort` (string?) — Sort order: `nameAsc`, `nameDesc`, `priceAsc`, `priceDesc`
- **Example:** `/api/Product?categoryId=1&search=para&sort=priceAsc&pageIndex=1&pageSize=10`

**POST/PUT Form Fields:** `Name`, `Description`, `Price`, `Stock`, `CategoryId`, `Image` (file)

> **Note:** Product images are stored in `wwwroot/images/products/`. On update, the old image is deleted when a new one is uploaded. On delete, the image file is also removed.

#### 📁 Category Endpoints

| Method | Endpoint | Description | Content-Type |
|--------|----------|-------------|--------------|
| `GET` | `/api/Category` | Get all categories | — |
| `GET` | `/api/Category/{id}` | Get category by ID | — |
| `POST` | `/api/Category` | Create a new category | `multipart/form-data` |
| `PUT` | `/api/Category/{id}` | Update a category | `multipart/form-data` |
| `DELETE` | `/api/Category/{id}` | Delete a category | — |

**POST/PUT Form Fields:** `NameAr`, `NameEn`, `NameRu`, `Image` (file)

> **Note:** Category images are stored in `wwwroot/images/categories/`. On update, the old image is deleted when a new one is uploaded. On delete, the image file is also removed.

#### 🛒 Cart Endpoints

| Method | Endpoint | Description | Content-Type |
|--------|----------|-------------|--------------|
| `GET` | `/api/cart/{cartId}` | Get cart by ID | — |
| `POST` | `/api/cart/{cartId}/items` | Add item to cart | `application/json` |
| `PUT` | `/api/cart/{cartId}/items/{productId}` | Update item quantity | `application/json` |
| `DELETE` | `/api/cart/{cartId}/items/{productId}` | Remove item from cart | — |
| `DELETE` | `/api/cart/{cartId}` | Clear cart | — |

**POST/PUT JSON Body:** `ProductId` (for POST only), `Quantity`

> **Note:** Cart IDs are generated as UUID strings. If a cart doesn't exist, it's created automatically when the first item is added. Carts return calculated subtotals with live product prices.

#### 🔐 Authentication Endpoints

| Method | Endpoint | Description | Content-Type |
|--------|----------|-------------|--------------|
| `POST` | `/api/Account/login` | Login user and get JWT | `application/json` |
| `POST` | `/api/Account/register` | Register a new user | `application/json` |
| `GET` | `/api/Account` | Get current logged-in user | — |

**Login JSON Body:** `Email`, `Password`, `CartId` (optional)
**Register JSON Body:** `FirstName`, `LastName`, `Email`, `PhoneNumber`, `Password`

> **Note:** Accessing the `GET /api/Account` endpoint requires passing the JWT token in the `Authorization` header as `Bearer <token>`.

#### 🛡️ Admin Users Endpoints

| Method | Endpoint | Description | Content-Type |
|--------|----------|-------------|--------------|
| `GET` | `/api/AdminUsers` | Get all users with their roles | — |
| `POST` | `/api/AdminUsers/promote/{userId}` | Promote a user to Admin role | — |
| `POST` | `/api/AdminUsers/demote/{userId}` | Demote an Admin to regular user | — |

> **Note:** All `/api/AdminUsers` endpoints require `Admin` role authorization.
