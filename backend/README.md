# Shipping Company Management API

ASP.NET Core Web API backend for a shipping company management course design.

## Stack

- ASP.NET Core 8 Web API
- EF Core 8
- SQL Server
- JWT authentication
- Swagger/OpenAPI

## Setup

1. Install the .NET 8 SDK.
2. Make sure SQL Server is running.
3. Update `appsettings.Development.json` if your SQL Server instance is not `localhost`.
4. Run the API:

```powershell
dotnet restore
dotnet run
```

The API opens Swagger at:

```text
https://localhost:7188/swagger
http://localhost:5188/swagger
```

On startup, the app applies the initial EF Core migration and seeds demo data.

## Demo Accounts

All demo accounts use the password `123456`.

| Username | Role |
| --- | --- |
| admin | Admin |
| dispatcher | Dispatcher |
| finance | Finance |
| viewer | Viewer |

## Main API Groups

- `POST /api/auth/login`
- `GET /api/auth/me`
- `GET/POST/PUT/DELETE /api/users`
- `GET/POST/PUT/DELETE /api/vessels`
- `GET/POST/PUT/DELETE /api/routes`
- `GET/POST/PUT/DELETE /api/customers`
- `GET/POST/PUT/DELETE /api/cargoes`
- `GET/POST/PUT/DELETE /api/orders`
- `POST /api/orders/{id}/dispatch`
- `PATCH /api/orders/{id}/status`
- `PATCH /api/orders/{id}/progress`
- `POST /api/orders/{id}/settlement`
- `GET /api/settlements`
- `POST /api/settlements/{id}/payments`
- `GET /api/dashboard/summary`
- `GET /api/system/backups`
- `POST /api/system/backups`

## Frontend Integration

The default CORS policy allows Vue 3 dev server requests from `http://localhost:5173`.
