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

| Variable | Description | Example |
|----------|-------------|---------|
| `ConnectionStrings__Default` | PostgreSQL DSN | `Host=postgres;Username=appuser;Password=secret;Database=appdb` |
| `Redis__ConnectionString` | Redis connection string | `redis` or `localhost:6379` |

## Run with Docker Compose

```sh
docker compose up -d --build
```

## Test

```sh
curl http://localhost:8000/
curl http://localhost:8000/counter
curl -X POST http://localhost:8000/counter
curl http://localhost:8000/counter-redis
curl -X POST http://localhost:8000/counter-redis
curl http://localhost:8000/health
```

## Teardown

```sh
docker compose down
```
