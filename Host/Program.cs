using Auth.Api;
using Auth.Application;
using Auth.Infrastructure;
using Auth.Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthApplication();
builder.Services.AddAuthInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

DapperConfiguration.RegisterTypeHandlers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthModule();

app.Run();