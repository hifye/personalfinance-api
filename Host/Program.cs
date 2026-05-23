using System.Text.Json;
using System.Text.Json.Serialization;
using Auth.Api;
using Auth.Application;
using Auth.Infrastructure;
using BuildingBlocks;
using BuildingBlocks.Configurations;
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

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
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

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString);

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

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler("/error");

// Mapeia Modulos 
app.MapAuthModule();
app.MapCatalogModule();
app.MapFinanceModule();
app.MapHealthChecks("/health");

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