using LastStand.APIBase;

var builder = WebApplication.CreateBuilder(args);

builder.SetupAPI("my-super-duper-secret-key-for-tokens");
builder.SetupOrleansHost("***table-storage-connectionstring***", "***redis-cache-connectionstring***");

var app = builder.Build();

app.SetupApplication();

app.Run();
