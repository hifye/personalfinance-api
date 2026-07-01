using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Auth.Api;
using Auth.Application;
using Auth.Infrastructure;
using BuildingBlocks;
using BuildingBlocks.Configurations;
using BuildingBlocks.Middlewares;
using Catalog.Api;
using Catalog.Application;
using Catalog.Infrastructure;
using Finance.Api;
using Finance.Application;
using Finance.Infrastructure;
using Host;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web host");
    // Registra os handlers de tipos de dados
    DapperConfiguration.RegisterTypeHandlers();

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("ApplicationName", "PersonalFinance");
    });

    var connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "DefaultConnection string was not configured."
        );

// Configura o JSON
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        );
    });

    builder.Services.AddRateLimiter(opt =>
    {
        opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        opt.AddPolicy("auth-strict", httpContext => RateLimitPartition.GetFixedWindowLimiter(
            GetClientIp(httpContext), _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }
        ));

        opt.AddPolicy("auth-token", httpContext => RateLimitPartition.GetFixedWindowLimiter(
            GetClientIp(httpContext), _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }
        ));

        opt.AddPolicy("writes", httpContext => RateLimitPartition.GetFixedWindowLimiter(
            GetAuthenticatedClientKey(httpContext), _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }
        ));

        opt.AddPolicy("heavy-reads", httpContext => RateLimitPartition.GetFixedWindowLimiter(
            GetAuthenticatedClientKey(httpContext), _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }
        ));

        opt.AddPolicy("general-authenticated", httpContext => RateLimitPartition.GetFixedWindowLimiter(
            GetAuthenticatedClientKey(httpContext), _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }
        ));

        opt.AddPolicy("health", httpContext => RateLimitPartition.GetFixedWindowLimiter(
            GetClientIp(httpContext), _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }
        ));
    });

    static string GetClientIp(HttpContext context)
    {
        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwarded))
        {
            var clientIp = forwarded.Split(',')[0].Trim();
            return clientIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    static string GetAuthenticatedClientKey(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
            return $"user:{userId}";

        return $"ip:{GetClientIp(context)}";
    }

    builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
        .AddNpgSql(connectionString, tags: ["ready"]);

// Auth
    builder.Services.AddAuthApplication();
    builder.Services.AddAuthInfrastructure(builder.Configuration);

// Catalog
    builder.Services.AddCatalogApplication();
    builder.Services.AddCatalogInfrastructure();

// Finance
    builder.Services.AddFinanceApplication();
    builder.Services.AddFinanceInfrastructure();

// Host
    builder.Services.AddHost();

// BuildingBlocks registra MediatR, behaviors, UoW, CurrentUser
// Recebe os assemblies de TODOS os módulos Application
    builder.Services.AddBuildingBlocks(
        Auth.Application.AssemblyReference.Assembly,
        Catalog.Application.AssemblyReference.Assembly,
        Finance.Application.AssemblyReference.Assembly
    );

    builder.Services.AddOpenApi();
    builder.Services.AddAuthorization();

    var app = builder.Build();

    app.UseForwardedHeaders(new ForwardedHeadersOptions()
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    if (app.Environment.IsDevelopment())
        app.MapOpenApi();

    app.MapScalarApiReference();

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("User", httpContext.User.Identity?.Name ?? "Anonymous");
            diagnosticContext.Set("RemoteIp", httpContext.Connection.RemoteIpAddress?.ToString());
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };
    });

    app.UseCorrelationId();

    app.UseExceptionHandler("/error");

    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseAuthorization();

// Mapeia Modulos 
    app.MapAuthModule();
    app.MapCatalogModule();
    app.MapFinanceModule();
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live")
    });
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });
    app.MapHealthChecks("/health")
        .RequireRateLimiting("health");

    app.Run();
}
catch (Exception ex) when (ex is not OperationCanceledException)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{
}