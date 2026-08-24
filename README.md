# Streaming API

A backend API for an anime catalogue / streaming-platform service, built with **Clean Architecture** on **.NET 10**. It handles the anime/season/episode catalogue, user accounts, watchlists, favorites and ratings.

> **Status:** work in progress, not yet production-ready. See [Known Limitations](#known-limitations) before deploying it anywhere public.

## Table of Contents

- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Configuration](#configuration)
  - [Run everything with Docker Compose](#run-everything-with-docker-compose)
  - [Run locally without Docker](#run-locally-without-docker)
  - [Database migrations](#database-migrations)
- [Authentication](#authentication)
  - [Creating an Admin account](#creating-an-admin-account)
- [API Reference](#api-reference)
  - [Auth](#auth)
  - [Anime](#anime)
  - [Genre](#genre)
  - [Studio](#studio)
  - [Season](#season)
  - [Episode](#episode)
  - [Watchlist & Favorites](#watchlist--favorites)
  - [User Profile](#user-profile)
- [Roles](#roles)
- [Rate Limiting](#rate-limiting)
- [Running Tests](#running-tests)
- [Known Limitations](#known-limitations)

## Tech Stack

| Layer         | Technology |
|---------------|------------|
| **Runtime**   | ASP.NET Core 10 (.NET 10) |
| **Database**  | PostgreSQL 18 + Entity Framework Core 10 (Npgsql) |
| **CQRS**      | MediatR |
| **Validation**| FluentValidation |
| **Auth**      | Custom cookie-based session authentication (hand-rolled, **not** ASP.NET Core Identity) |
| **Testing**   | xUnit, NSubstitute, FluentAssertions, Testcontainers (PostgreSQL), `WebApplicationFactory` |

## Project Structure

```
WebApi.slnx
├── Domain/               # Entities, value objects, domain errors/exceptions, domain events
├── Application/          # CQRS commands/queries (MediatR handlers), FluentValidation validators
├── Authorization/        # Cookie auth configuration, security-stamp validation, current-user/claims providers
├── Infrastructure/       # Password hashing, time provider
├── Persistence/          # EF Core DbContext, entity configurations, repositories, migrations
├── WebApi/               # Controllers, middleware, Program.cs (composition root)
└── Tests/
    ├── Domain.Tests/         # entity + value object unit tests
    ├── Application.Tests/    # CQRS handler unit tests (NSubstitute mocks)
    └── Integration.Tests/    # full-pipeline API tests against a real Postgres via Testcontainers
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker (for PostgreSQL, and/or to run the whole stack in containers)

### Configuration

Copy `.env.example` to `.env` and adjust the values:

```env
POSTGRES_DB=streaming
POSTGRES_USER=postgres
POSTGRES_PASSWORD=pass12345
DB_CONNECTION_STRING=Server=streaming.database;Port=5432;Database=streaming;Username=postgres;Password=pass12345;
```

> ⚠️ These are local-development placeholders. Use strong, unique credentials — and inject them via your deployment platform's secrets/config, not a committed file — anywhere outside your own machine.

### Run everything with Docker Compose

```bash
docker compose up -d
```

This builds and starts two services:

| Service | What it is | Ports |
|---|---|---|
| `webapi` | The API | `8080` (HTTP), `8081` (HTTPS) |
| `streaming.database` | PostgreSQL 18 | `5432` |

> **Note:** `docker-compose.override.yml` is loaded automatically by Docker Compose and is set up for local Visual Studio debugging on Windows (it mounts `%APPDATA%` paths for user-secrets/dev HTTPS certs and forces `ASPNETCORE_ENVIRONMENT=Development`). On macOS/Linux, in CI, or for any non-Windows/non-dev run, ignore or delete it and supply your own compose override instead — otherwise `docker compose up` may fail or start the API in Development mode.

### Run locally without Docker

```bash
dotnet restore WebApi.slnx
dotnet run --project WebApi
```

The API listens on `http://localhost:5256` (and `https://localhost:7228` for the `https` launch profile) — see `WebApi/Properties/launchSettings.json`.

### Database migrations

Migrations are applied automatically on startup **only** when `ASPNETCORE_ENVIRONMENT=Development`, or when the `AutoMigrate` configuration value is `true`. In any other environment, apply them explicitly as part of your deploy:

```bash
dotnet ef database update --project Persistence --startup-project WebApi
```

## Authentication

Auth is a custom cookie-session implementation (not ASP.NET Core Identity):

- `POST /auth/sign-in` / `POST /auth/sign-up` set an `httpOnly`, `Secure`, `SameSite=None` cookie named `streaming-auth`. `SameSite=None` means the frontend must call the API with credentials included (e.g. `fetch(..., { credentials: 'include' })`), and the API's CORS policy must explicitly allow that exact origin with `AllowCredentials()`.
- The session is re-validated against the user's security stamp at most every 10 minutes. `POST /auth/sign-out-all` rotates the stamp, which immediately invalidates every other active session on its next check.
- Unauthenticated requests to protected routes get `401`; authenticated-but-wrong-role requests get `403`.

### Creating an Admin account

There is currently **no built-in way** to create an Admin user — every self-registered account gets the `User` role, and nothing in the codebase ever grants `Admin`. Until that's built, promote a user manually after they sign up:

```sql
UPDATE "Users" SET "Role" = 'Admin' WHERE "Email" = 'you@example.com';
```

They'll need to sign out and back in (or wait for the next security-stamp check) for the new role to take effect.

## API Reference

All endpoints accept/return `application/json`. Errors follow the RFC 7807 `ProblemDetails` shape (`title`, `detail`, `status`, and an `errors` extension for validation failures).

### Auth

| Method | Path | Auth | Body | Description |
|--------|------|:---:|------|-------------|
| POST | `/auth/sign-up` | – | `{ email, username, password, rememberMe }` | Register and sign in |
| POST | `/auth/sign-in` | – | `{ usernameOrEmail, password, rememberMe }` | Sign in |
| POST | `/auth/sign-out` | ✓ | – | Sign out the current session |
| POST | `/auth/sign-out-all` | ✓ | – | Sign out every session (rotates the security stamp) |
| GET | `/auth/me` | ✓ | – | Current user's id/username/email/role |
| POST | `/auth/change-password` | ✓ | `{ currentPassword, newPassword }` | Change password |

`sign-up`, `sign-in` and `change-password` are rate-limited (see [Rate Limiting](#rate-limiting)). Sign-up/sign-in/sign-out return an empty `200 OK` body — the session lives in the cookie.

### Anime

| Method | Path | Auth | Description |
|--------|------|:---:|-------------|
| POST | `/api/anime` | Admin | Create anime |
| GET | `/api/anime/{id}` | – | Get anime by id |
| GET | `/api/anime` | – | List anime — see query params below |
| PUT | `/api/anime/{id}` | Admin | Update anime |
| PUT | `/api/anime/{animeId}/rate` | ✓ | Rate an anime (`1.0`–`10.0`) |
| DELETE | `/api/anime/{id}` | Admin | Delete anime (**permanent**, cascades to its seasons/episodes and every user's watchlist/favorite/rating entries for it) |

`GET /api/anime` query params: `page` (default `1`), `pageSize` (default `20`, capped at `100`), `search`, `genre`, `status`, `releaseYear`, `studio`, `fromDate`, `toDate`, `sortBy` (`title` \| `rating` \| `year` \| `created`), `sortOrder` (`asc` \| `desc`).

`AnimeStatus` values: `Announced`, `Airing`, `Completed`, `Hiatus`, `Upcoming`.

Create/update body:

```json
{
  "title": "Frieren: Beyond Journey's End",
  "originalTitle": "葬送のフリーレン",
  "englishTitle": "Frieren: Beyond Journey's End",
  "description": "...",
  "releaseYear": 2023,
  "status": "Completed",
  "coverImageUrl": "https://example.com/cover.jpg",
  "bannerImageUrl": "https://example.com/banner.jpg",
  "trailerUrl": "https://example.com/trailer.mp4",
  "ageRating": "PG-13",
  "genres": ["Adventure", "Drama"],
  "studios": ["Madhouse"]
}
```

`genres`/`studios` are matched **by exact name** against existing Genre/Studio records — create those first, or the request fails with `404`.

### Genre

| Method | Path | Auth | Description |
|--------|------|:---:|-------------|
| POST | `/api/genres` | Admin | Create genre — `{ name, description? }` |
| GET | `/api/genres/{id}` | – | Get genre by id |
| GET | `/api/genres?page=&pageSize=` | – | List genres (default `pageSize` 50) |
| PUT | `/api/genres/{id}` | Admin | Update genre |
| DELETE | `/api/genres/{id}` | Admin | Delete genre |

### Studio

| Method | Path | Auth | Description |
|--------|------|:---:|-------------|
| POST | `/api/studios` | Admin | Create studio — `{ name, description? }` |
| GET | `/api/studios/{id}` | – | Get studio by id |
| GET | `/api/studios?page=&pageSize=` | – | List studios (default `pageSize` 50) |
| PUT | `/api/studios/{id}` | Admin | Update studio |
| DELETE | `/api/studios/{id}` | Admin | Delete studio |

### Season

| Method | Path | Auth | Description |
|--------|------|:---:|-------------|
| POST | `/api/anime/{animeId}/seasons` | Admin | Create season — `{ animeId, seasonNumber, title, description }` |
| GET | `/api/seasons/{id}` | – | Get season by id |
| GET | `/api/anime/{animeId}/seasons` | – | List seasons for an anime |
| PUT | `/api/seasons/{id}` | Admin | Update season |
| DELETE | `/api/seasons/{id}` | Admin | Delete season |

### Episode

| Method | Path | Auth | Description |
|--------|------|:---:|-------------|
| POST | `/api/seasons/{seasonId}/episodes` | Admin | Create episode — `{ seasonId, episodeNumber, title, duration }` |
| GET | `/api/episodes/{id}` | – | Get episode by id |
| GET | `/api/seasons/{seasonId}/episodes` | – | List episodes for a season |
| PUT | `/api/episodes/{id}` | Admin | Update episode — `{ id, title, duration, description?, videoUrl, thumbnailUrl? }` |
| POST | `/api/episodes/{id}/publish` | Admin | Publish an episode — `{ id, releaseDate? }` (defaults to now) |
| DELETE | `/api/episodes/{id}` | Admin | Delete episode |

`duration` is a `TimeSpan`, serialized as e.g. `"00:24:00"` for 24 minutes. A new episode has no `videoUrl`/`thumbnailUrl` and `isPublished: false` until explicitly updated and published.

### Watchlist & Favorites

All routes below require authentication and act on the **current user only** (there's no way to view or modify another user's list).

| Method | Path | Description |
|--------|------|-------------|
| POST | `/anime/{animeId}/watchlist` | Add anime to watchlist — `{ animeId, status }` |
| PUT | `/anime/{animeId}/watchlist` | Update an entry — `{ animeId, status?, lastWatchedEpisodeNumber?, progressPercentage?, userRating?, isFavorite?, notes? }` (all fields but `animeId` optional/partial-update) |
| DELETE | `/anime/{animeId}/watchlist` | Remove from watchlist |
| GET | `/users/me/watchlist` | Get the current user's watchlist |
| POST | `/anime/{animeId}/favorite` | Toggle favorite on/off |
| DELETE | `/anime/{animeId}/favorite` | Remove from favorites |
| GET | `/users/me/favorites` | Get the current user's favorites |

`WatchStatus` values: `Planned`, `Watching`, `Completed`, `OnHold`, `Dropped`.

### User Profile

| Method | Path | Description |
|--------|------|-------------|
| GET | `/users/me` | Get the current user's profile (same data as `/auth/me`) |
| PUT | `/users/me` | Update profile — `{ username?, bio?, avatarUrl? }` |

## Roles

- **Admin** — manages the catalogue (anime, genres, studios, seasons, episodes). No self-service way to obtain this role — see [Creating an Admin account](#creating-an-admin-account).
- **User** — default role; manages their own profile, watchlist and favorites, and can rate anime.

## Rate Limiting

`/auth/sign-in`, `/auth/sign-up` and `/auth/change-password` are limited to **5 requests/minute per client IP** (fixed window, requests beyond the limit get `429 Too Many Requests`). No other endpoint is currently rate-limited.

## Running Tests

```bash
# All tests
dotnet test WebApi.slnx

# Individual projects
dotnet test Tests/Domain.Tests
dotnet test Tests/Application.Tests
dotnet test Tests/Integration.Tests   # requires Docker — spins up a disposable Postgres via Testcontainers
```

## Known Limitations

Things to be aware of before relying on this in a real deployment:

- **No Admin bootstrap** — see [above](#creating-an-admin-account); currently requires a manual DB update.
- **No email verification** on sign-up.
- **`IsBanned`/`BannedUntilUtc`** exist on the `User` entity and in the database, but nothing sets or checks them yet — banning doesn't actually work.
- **Deletes are permanent and cascading** — there's no soft-delete, confirmation step, or undo for `DELETE` on anime/season/episode, despite `IsActive` fields existing on those entities (they're currently never set to `false` by any code path).
- **CORS origin is hardcoded** to `http://localhost:5173` in `Program.cs` — update this per environment before deploying.
- **No video infrastructure** — `videoUrl` is just a validated link to an externally hosted file. There's no transcoding, adaptive bitrate streaming (HLS/DASH), DRM, or CDN integration in this repository; actual video delivery is expected to be handled elsewhere.
