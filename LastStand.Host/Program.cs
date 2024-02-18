using LastStand.APIBase;

var builder = WebApplication.CreateBuilder(args);

builder.SetupOrleansHost("***table-storage-connectionstring***", "***redis-cache-connectionstring***");

builder.Host.UseConsoleLifetime();

var app = builder.Build();

app.Run();