[README.md](https://github.com/user-attachments/files/26632683/README.md)
# 🔧 Master — Service-Worker Marketplace API

**Master** is a full-featured RESTful API for a service-worker marketplace platform, built with **ASP.NET Core 8** and following **Clean Architecture** principles. It connects **Clients** (who need services) with **Masters** (skilled professionals), enabling job posting, skill management, and a rating & review system.

---

## 📐 Architecture

The project follows **Clean Architecture** with strict separation of concerns:

```
Master/
├── Master.Domain          # Entities, Enums, Constants (zero dependencies)
├── Master.Application     # DTOs, Interfaces, CQRS Handlers, Validators, Mapping
├── Master.Infrastructure  # EF Core, Identity, Repositories, JWT, Hangfire
├── Master.API             # Controllers, Middleware, Extensions, Pipeline
└── Master.UnitTests       # xUnit-based unit tests
```

### Key Patterns
- **CQRS** with **MediatR** — Commands and Queries are strictly separated
- **Repository Pattern** — All data access is abstracted behind interfaces
- **AutoMapper** — Entity ↔ DTO mapping
- **FluentValidation** — Request validation with reusable rules

---

## 🛠 Tech Stack

| Layer | Technology |
|---|---|
| **Runtime** | .NET 8 |
| **Web Framework** | ASP.NET Core Web API |
| **ORM** | Entity Framework Core (SQL Server) |
| **Authentication** | ASP.NET Identity + JWT (Access + Refresh tokens) |
| **Authorization** | Role-based policies (Admin, Master, Client) |
| **Validation** | FluentValidation |
| **Mapping** | AutoMapper |
| **CQRS** | MediatR |
| **Background Jobs** | Hangfire (token cleanup) |
| **API Docs** | Swagger / OpenAPI |
| **Containerization** | Docker |

---

## 👤 Domain Models

### AppUser
Extends `IdentityUser<Guid>` with profile fields:
- `FirstName`, `LastName`, `Address`, `PhoneNumber`
- `Experience` (years), `Age`, `DateOfBirth`
- `AverageRating`, `RatingCount` — auto-calculated from reviews
- Navigation: `UserSkills`, `JobPosts`, `ReceivedRatings`, `GivenRatings`

### Skill
Professional abilities (e.g., Plumbing, Electrical Repair, Painting):
- `Name`, `Description`
- Many-to-many with `AppUser` via `UserSkill`

### JobPost
Service requests posted by Clients:
- `Title`, `Description`, `Budget`
- `JPStatus`: `Pending` → `Active` → `InProgress` → `Completed` / `Canceled`
- Linked to a `Customer` (Client) and a `RequiredSkill`

### MasterRating
Review system — Clients rate Masters:
- `MasterId`, `CustomerId` (composite key)
- `Score` (decimal), `Comment`
- Auto-syncs `AverageRating` on the Master's profile

---

## 🔑 Roles & Authorization

| Role | Description |
|---|---|
| **Admin** | Full system access, skill CRUD, user management |
| **Master** | Service providers — can manage skills on their profile |
| **Client** | Customers — can post jobs and rate masters |

### Policies
- `AdminOnly` — Admin role required
- `MasterOnly` — Master role required
- `ClientOnly` — Client role required
- `MasterOrAdmin` — Master or Admin
- `ClientOrAdmin` — Client or Admin

---

## 📡 API Endpoints

### 🔐 Auth (`api/auth`)

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/register` | ❌ | Register a new user (Master/Client) |
| `POST` | `/login` | ❌ | Login and receive JWT tokens |
| `POST` | `/details` | ❌ | Get user profile by ID (body) |
| `POST` | `/refresh` | ✅ | Refresh access token |
| `POST` | `/revoke` | ✅ | Revoke refresh token (logout) |
| `PUT` | `/editProfile` | ✅ | Update own profile |
| `PUT` | `/changePassword` | ✅ | Change own password |
| `DELETE` | `/deleteProfile` | ✅ | Permanently delete own account |
| `GET` | `/masters` | ✅ Client/Admin | Paged list of masters (search + rank sort) |
| `GET` | `/clients` | ✅ Master/Admin | Paged list of clients (search + name sort) |

### 💼 JobPost (`api/jobpost`)

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/paged` | ❌ | Paged job list (search, filter, sort) |
| `GET` | `/all` | ❌ | Get all jobs |
| `GET` | `/{id}` | ❌ | Get job details by ID |
| `GET` | `/myJobs` | ✅ | Get own active jobs |
| `GET` | `/user/{userId}` | ❌ | Get jobs by a specific user |
| `GET` | `/{id}/owner` | ❌ | Get job owner's profile |
| `GET` | `/bySkill/{skillId}` | ❌ | Get active jobs by required skill |
| `GET` | `/statuses` | ✅ | Get all possible job statuses |
| `POST` | `/create` | ✅ | Create a new job post |
| `PUT` | `/{id}` | ✅ | Update a job post |
| `PATCH` | `/{id}/status` | ✅ | Change job status |
| `DELETE` | `/{id}` | ✅ | Delete own job post |

### 🛠 Skill (`api/skill`)

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/all` | ✅ | Get all skills |
| `GET` | `/paged` | ✅ | Paged skill list (search, sort) |
| `GET` | `/my-skills` | ✅ | Get own skills |
| `POST` | `/` | ✅ Admin | Create a new skill |
| `PUT` | `/{id}` | ✅ Admin | Update a skill |
| `POST` | `/assignMe` | ✅ Master/Admin | Assign skills to own profile |
| `DELETE` | `/removeSkill/{skillId}` | ✅ Master/Admin | Remove a skill from own profile |

### ⭐ MasterRating (`api/masterrating`)

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/{masterId}` | ✅ | Get all ratings for a master |
| `POST` | `/` | ✅ | Create a rating for a master |
| `PUT` | `/` | ✅ | Update an existing rating |
| `DELETE` | `/{masterId}/{customerId}` | ✅ | Delete a rating |

---

## ⚙️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) (LocalDB or full instance)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/Master.git
   cd Master
   ```

2. **Configure the database connection**
   
   Update `appsettings.json` in `Master.API`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnectionString": "Server=localhost;Database=MasterDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Apply migrations**
   ```bash
   dotnet ef database update --project Master.Infrastructure --startup-project Master.API
   ```

4. **Run the application**
   ```bash
   dotnet run --project Master.API
   ```

5. **Open Swagger UI**
   
   Navigate to: `https://localhost:{port}/swagger`

### Seed Data (Optional)
Uncomment the following line in `Program.cs` to populate the database with sample data:
```csharp
await app.EnsureSeededAsync();
```
This will create:
- Default Admin, 20 Masters, and 20 Clients
- 10 predefined skills
- Random job posts and ratings

---

## 🐳 Docker

```bash
docker build -t master-api .
docker run -p 8080:8080 master-api
```

---

## 🧪 Testing

```bash
dotnet test Master.UnitTests
```

---

## 📁 Key Files

| File | Purpose |
|---|---|
| `Program.cs` | Application entry point and service registration |
| `ServiceCollectionExtensions.cs` | DI registration for all services |
| `PipelineExtensions.cs` | Middleware pipeline configuration |
| `GlobalExceptionMiddleware.cs` | Centralized error handling |
| `MasterDbContext.cs` | EF Core context with Fluent API configuration |
| `RoleSeeder.cs` | Database seeding for roles, users, skills, ratings |
| `MappingProfile.cs` | AutoMapper configuration |
| `TokenService.cs` | JWT access + refresh token generation and validation |

---

## 📄 License

This project is licensed under the [MIT License](https://opensource.org/license/mit).
