using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using StackExchange.Redis;
using LastStand.Dashboard;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace LastStand.APIBase;
public static class ApiExtensions
{
    public static void SetupAPI(this WebApplicationBuilder webBuilder, string tokenKey)
    {
        webBuilder.Services.AddControllers();
        webBuilder.Services.AddEndpointsApiExplorer();
        webBuilder.Services.AddSwaggerGen();
        var key = Encoding.ASCII.GetBytes(tokenKey);
        webBuilder.Services.AddKeyedSingleton("token-key", key);
        webBuilder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        webBuilder.Services.AddSwaggerGen(options =>
        {
            // add JWT Authentication
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, new string[] { } }
            });
        });
    }

    public static void SetupApplication(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.SetupDashboard();
    }

    public static void SetupOrleansClient(this WebApplicationBuilder webBuilder, string tableStorageConnectionString)
    {
        webBuilder.Host.UseOrleansClient(builder =>
        {
            builder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "last-stand-cluster";
                options.ServiceId = "last-stand";
            });
#if DEBUG
            builder.UseLocalhostClustering();
#else
            builder.UseAzureStorageClustering(x => x.ConfigureTableServiceClient(tableStorageConnectionString));
#endif
        });
    }

    public static void SetupOrleansHost(this WebApplicationBuilder webBuilder, string tableStorageConnectionString, string cacheConnectionString)
    {
        webBuilder.Host.UseOrleans(builder =>
        {
            builder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "last-stand-cluster";
                options.ServiceId = "last-stand";
            });

#if DEBUG
            builder.UseLocalhostClustering();
            builder.AddMemoryGrainStorage("laststandstore");
#else
            builder.UseAzureStorageClustering(x => x.ConfigureTableServiceClient(tableStorageConnectionString));
            builder.ConfigureEndpoints(11111, 30000);
            builder.AddAzureTableGrainStorage(
                name: "laststandstore",
                configureOptions: options =>
                {
                    options.ConfigureTableServiceClient(tableStorageConnectionString);
                });
#endif
        });
        webBuilder.SetupCache(cacheConnectionString);
    }

    public static void SetupCache(this WebApplicationBuilder webBuilder, string cacheConnectionString)
    {
        webBuilder.Services.AddSingleton(ConnectionMultiplexer.Connect(cacheConnectionString));
    }
}
