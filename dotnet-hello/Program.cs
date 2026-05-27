using Npgsql;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var dsn = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured");
var redisUrl = builder.Configuration["Redis:ConnectionString"]
    ?? throw new InvalidOperationException("Redis:ConnectionString is not configured");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(dsn);
dataSourceBuilder.ConnectionStringBuilder.Pooling = true;
dataSourceBuilder.ConnectionStringBuilder.MinPoolSize = 2;
dataSourceBuilder.ConnectionStringBuilder.MaxPoolSize = 10;
builder.Services.AddSingleton<NpgsqlDataSource>(dataSourceBuilder.Build());

var redisConfig = ConfigurationOptions.Parse(redisUrl);
redisConfig.AbortOnConnectFail = false;
builder.Services.AddSingleton<IConnectionMultiplexer>(await ConnectionMultiplexer.ConnectAsync(redisConfig));

var app = builder.Build();

app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Served-From"] = Environment.MachineName;
    await next();
});

// Run migration on startup
await using (var scope = app.Services.CreateAsyncScope())
{
    var dataSource = scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>();
    await using var conn = await dataSource.OpenConnectionAsync();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = """
        CREATE TABLE IF NOT EXISTS counter (
            count INT NOT NULL DEFAULT 0,
            last_hostname TEXT NOT NULL DEFAULT '',
            updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
            created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
        );
        INSERT INTO counter (count, last_hostname)
        SELECT 0, ''
        WHERE NOT EXISTS (SELECT 1 FROM counter);
        """;
    await cmd.ExecuteNonQueryAsync();
}

app.MapGet("/", () => $"Hello from {Environment.MachineName}");

app.MapGet("/counter", async (NpgsqlDataSource dataSource) =>
{
    await using var conn = await dataSource.OpenConnectionAsync();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT count, last_hostname, updated_at, created_at FROM counter";
    await using var reader = await cmd.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
        return Results.Ok(new
        {
            count = reader.GetInt32(0),
            last_hostname = reader.GetString(1),
            updated_at = reader.GetDateTime(2),
            created_at = reader.GetDateTime(3)
        });
    }
    return Results.NotFound();
});

app.MapPost("/counter", async (NpgsqlDataSource dataSource) =>
{
    var hostname = Environment.MachineName;
    await using var conn = await dataSource.OpenConnectionAsync();
    await using var tx = await conn.BeginTransactionAsync();

    await using var cmd = conn.CreateCommand();
    cmd.Transaction = tx;
    cmd.CommandText = """
        UPDATE counter
        SET count = count + 1, last_hostname = @hostname, updated_at = NOW()
        RETURNING count, last_hostname, updated_at, created_at
        """;
    cmd.Parameters.AddWithValue("hostname", hostname);
    await using var reader = await cmd.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
        var result = new
        {
            count = reader.GetInt32(0),
            last_hostname = reader.GetString(1),
            updated_at = reader.GetDateTime(2),
            created_at = reader.GetDateTime(3)
        };
        await reader.CloseAsync();
        await tx.CommitAsync();
        return Results.Ok(result);
    }
    await tx.CommitAsync();
    return Results.NotFound();
});

app.MapGet("/counter-redis", (IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var entries = db.HashGetAll("counter");
    if (entries.Length == 0)
    {
        return Results.Ok(new { count = 0, last_hostname = (string?)null, updated_at = (string?)null, created_at = (string?)null });
    }
    var dict = entries.ToDictionary(e => (string)e.Name!, e => (string?)e.Value);
    return Results.Ok(new
    {
        count = int.Parse(dict.GetValueOrDefault("count", "0")!),
        last_hostname = dict.GetValueOrDefault("last_hostname"),
        updated_at = dict.GetValueOrDefault("updated_at"),
        created_at = dict.GetValueOrDefault("created_at")
    });
});

app.MapPost("/counter-redis", (IConnectionMultiplexer redis) =>
{
    var hostname = Environment.MachineName;
    var db = redis.GetDatabase();
    var newCount = db.HashIncrement("counter", "count");
    db.HashSet("counter", [new HashEntry("last_hostname", hostname), new HashEntry("updated_at", DateTime.UtcNow.ToString("o"))]);
    if (newCount == 1)
    {
        db.HashSet("counter", "created_at", DateTime.UtcNow.ToString("o"));
    }
    return Results.Ok(new { count = (long)newCount });
});

app.MapGet("/health", async (NpgsqlDataSource dataSource, IConnectionMultiplexer redis) =>
{
    string pgStatus;
    try
    {
        await using var cmd = dataSource.CreateCommand("SELECT 1");
        await cmd.ExecuteScalarAsync();
        pgStatus = "healthy";
    }
    catch
    {
        pgStatus = "unhealthy";
    }

    var redisStatus = redis.IsConnected ? "healthy" : "unhealthy";

    return Results.Ok(new
    {
        hostname = Environment.MachineName,
        pgsql = pgStatus,
        redis = redisStatus
    });
});

app.Run("http://*:8000");
