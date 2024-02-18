using LastStand.APIBase;

var builder = WebApplication.CreateBuilder(args);

builder.SetupAPI("my-super-duper-secret-key-for-tokens");
builder.SetupOrleansClient("***table-storage-connectionstring***");

var app = builder.Build();

app.SetupApplication();

app.Run();
