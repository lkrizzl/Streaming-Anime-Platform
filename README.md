# Sreaming API

A backend API for an anime streaming service built with **Clean Architecture** in **.NET 10**.

## Tech Stack

| Layer        | Technology |
|-------------|-----------|
| **Runtime** | ASP.NET Core 10 (NET 10.0) |
| **Database** | PostgreSQL + Entity Framework Core 10 |
| **CQRS** | MediatR |
| **Validation** | FluentValidation |
| **Auth** | Cookie authentication (ASP.NET Core Identity) |
| **API docs** | Swashbuckle (Swagger) |
| **Testing** | xUnit, NSubstitute, Testcontainers (PostgreSQL), WebApplicationFactory |

## Project Structure

```
WebApi.slnx
├── Domain/               # Entities, ValueObjects, Enums, Exceptions
├── Application/          # CQRS handlers, DTOs, validators
├── Persistence/          # EF Core DbContext, repositories, migrations
├── Infrastructure/       # Shared services (time provider, etc.)
├── Authorization/        # Cookie auth configuration
├── WebApi/               # Controllers, middleware, Program.cs
└── Tests/
    ├── Domain.Tests/         # 118 unit tests (entities + value objects)
    ├── Application.Tests/    # 25 unit tests (CQRS handlers)
    └── Integration.Tests/    # 19 integration tests (API + Testcontainers)
```

## Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL)

### 1. Start PostgreSQL

```bash
docker run -d --name streaming-db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=streaming \
  -p 5432:5432 \
  postgres:latest
```

### 2. Set up connection string

The connection string is stored in **User Secrets** (development only) or **environment variables** (production).

```bash
cd WebApi
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=streaming;Username=postgres;Password=postgres"
```

### 3. Apply migrations and run

```bash
dotnet ef database update --project Persistence --startup-project WebApi
dotnet run --project WebApi/WebApi.csproj
```

The API starts at `http://localhost:5256`.  
Swagger UI: [http://localhost:5256/swagger](http://localhost:5256/swagger)

### Docker Compose

```bash
docker compose up -d
```

Runs both the API (`webapi`) and PostgreSQL (`streaming-db`) together.

## API Endpoints

### Auth (`/auth`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/auth/sign-up` | — | Register a new user |
| POST | `/auth/sign-in` | — | Sign in |
| POST | `/auth/sign-out` | ✓ | Sign out current session |
| POST | `/auth/sign-out-all` | ✓ | Sign out all sessions |
| GET | `/auth/me` | ✓ | Get current user info |
| PUT | `/auth/change-password` | ✓ | Change password |

### Anime (`/api/anime`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/anime` | Admin | Create anime |
| GET | `/api/anime/{id}` | — | Get anime by ID |
| GET | `/api/anime?page=&pageSize=&search=&genre=&status=&sortBy=&sortOrder=` | — | List animes (filtered, sorted, paginated) |
| PUT | `/api/anime/{id}` | Admin | Update anime |
| DELETE | `/api/anime/{id}` | Admin | Delete anime |
| PUT | `/api/anime/{animeId}/rate` | ✓ | Rate an anime (1.0–10.0) |

### Genre (`/api/genre`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/genre` | Admin | Create genre |
| GET | `/api/genre/{id}` | — | Get genre by ID |
| GET | `/api/genre?page=&pageSize=` | — | List genres (paginated) |
| PUT | `/api/genre/{id}` | Admin | Update genre |
| DELETE | `/api/genre/{id}` | Admin | Delete genre |

### Studio (`/api/studio`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/studio` | Admin | Create studio |
| GET | `/api/studio/{id}` | — | Get studio by ID |
| GET | `/api/studio?page=&pageSize=` | — | List studios (paginated) |
| PUT | `/api/studio/{id}` | Admin | Update studio |
| DELETE | `/api/studio/{id}` | Admin | Delete studio |

### Season (`/api/anime/{animeId}/seasons`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/anime/{animeId}/seasons` | Admin | Create season |
| GET | `/api/seasons/{id}` | — | Get season by ID |
| GET | `/api/anime/{animeId}/seasons` | — | Get all seasons for an anime |
| PUT | `/api/seasons/{id}` | Admin | Update season |
| DELETE | `/api/seasons/{id}` | Admin | Delete season |

### Episode (`/api/seasons/{seasonId}/episodes`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/seasons/{seasonId}/episodes` | Admin | Create episode |
| GET | `/api/episodes/{id}` | — | Get episode by ID |
| GET | `/api/seasons/{seasonId}/episodes` | — | Get episodes by season |
| PUT | `/api/episodes/{id}` | Admin | Update episode |
| PUT | `/api/episodes/{id}/publish` | Admin | Publish episode |
| DELETE | `/api/episodes/{id}` | Admin | Delete episode |

### User Anime Watchlist (`/api/user-anime`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/user-anime` | ✓ | Add anime to watchlist |
| PUT | `/api/user-anime` | ✓ | Update watchlist entry |
| DELETE | `/api/user-anime/{animeId}` | ✓ | Remove from watchlist |
| GET | `/api/user-anime/watchlist` | ✓ | Get my watchlist |
| PUT | `/api/user-anime/{animeId}/favorite` | ✓ | Toggle favorite |
| GET | `/api/user-anime/favorites` | ✓ | Get my favorites |

### User Profile (`/api/users`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| GET | `/api/users/me` | ✓ | Get my profile |
| PUT | `/api/users/me` | ✓ | Update my profile (avatar, bio) |

## Roles

- **Admin** — manages the catalogue (anime, genres, seasons, episodes, studios)
- **User** — manages their own profile, watchlist, and favorites

## Rate Limiting

- **Auth endpoints** (sign-in, sign-up): 5 requests per minute per IP
- All other endpoints: unlimited

## Running Tests

```bash
# All tests
dotnet test WebApi.slnx

# Specific projects
dotnet test Tests/Domain.Tests
dotnet test Tests/Application.Tests
dotnet test Tests/Integration.Tests   # Requires Docker (Testcontainers)
```

> **Note:** Integration tests use [Testcontainers](https://testcontainers.com/) to spin up a disposable PostgreSQL container automatically. Docker Desktop must be running.
