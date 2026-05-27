# dotnet-hello

.NET 10 minimal web API with PostgreSQL and Redis backends.

## Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/` | Returns hostname |
| GET | `/counter` | Get PgSQL counter state |
| POST | `/counter` | Increment PgSQL counter |
| GET | `/counter-redis` | Get Redis counter state |
| POST | `/counter-redis` | Increment Redis counter |
| GET | `/health` | Health check (PgSQL + Redis status) |

All responses include `X-Served-From: <hostname>` header.

## Environment Variables

| Variable | Required | Description | Example |
|----------|----------|-------------|---------|
| `ConnectionStrings__Default` | yes | PostgreSQL DSN | `Host=dbserver;Username=appuser;Password=secret;Database=appdb` |
| `Redis__ConnectionString` | yes | Redis connection string | `kvserver:6379` |
| `ASPNETCORE_HTTP_PORTS` | no | Listen port (default: `8080`) | `8080` |

## Run with Docker Compose

```sh
docker compose up -d --build
```

## Test

```sh
curl http://localhost:8080/
curl http://localhost:8080/counter
curl -X POST http://localhost:8080/counter
curl http://localhost:8080/counter-redis
curl -X POST http://localhost:8080/counter-redis
curl http://localhost:8080/health
```

## Teardown

```sh
docker compose down
```
