using Auth.Api;
using Auth.Application;
using Auth.Infrastructure;
using BuildingBlocks;
using BuildingBlocks.Configurations;
using Catalog.Api;
using Catalog.Application;
using Catalog.Infrastructure;
using Host;

var builder = WebApplication.CreateBuilder(args);

// Auth
builder.Services.AddAuthApplication();
builder.Services.AddAuthInfrastructure(builder.Configuration);

// Catalog
builder.Services.AddCatalogApplication();
builder.Services.AddCatalogInfrastructure();

// Host
builder.Services.AddHost();

// BuildingBlocks registra MediatR, behaviors, UoW, CurrentUser
// Recebe os assemblies de TODOS os módulos Application
builder.Services.AddBuildingBlocks(
    Auth.Application.AssemblyReference.Assembly,
    Catalog.Application.AssemblyReference.Assembly
);

builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

// Registra os handlers de tipos de dados
DapperConfiguration.RegisterTypeHandlers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

// Mapeia Modulos 
app.MapAuthModule();
app.MapCatalogModule();

app.Run();