var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => $"Hello from {Environment.MachineName}");

app.Run("http://*:8000");
