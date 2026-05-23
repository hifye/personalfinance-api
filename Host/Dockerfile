FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Host/Host.csproj", "Host/"]
COPY ["src/Modules/Auth/Auth.Domain/Auth.Domain.csproj", "src/Modules/Auth/Auth.Domain/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
COPY ["src/Modules/Auth/Auth.Api/Auth.Api.csproj", "src/Modules/Auth/Auth.Api/"]
COPY ["src/Modules/Auth/Auth.Application/Auth.Application.csproj", "src/Modules/Auth/Auth.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/"]
COPY ["src/Modules/Auth/Auth.Infrastructure/Auth.Infrastructure.csproj", "src/Modules/Auth/Auth.Infrastructure/"]
COPY ["src/Modules/Finance/Finance.Infrastructure/Finance.Infrastructure.csproj", "src/Modules/Finance/Finance.Infrastructure/"]
COPY ["src/Modules/Finance/Finance.Application/Finance.Application.csproj", "src/Modules/Finance/Finance.Application/"]
COPY ["src/Modules/Catalog/Catalog.Application/Catalog.Application.csproj", "src/Modules/Catalog/Catalog.Application/"]
COPY ["src/Modules/Catalog/Catalog.Domain/Catalog.Domain.csproj", "src/Modules/Catalog/Catalog.Domain/"]
COPY ["src/Modules/Finance/Finance.Domain/Finance.Domain.csproj", "src/Modules/Finance/Finance.Domain/"]
COPY ["src/Modules/Finance/Finance.Api/Finance.Api.csproj", "src/Modules/Finance/Finance.Api/"]
COPY ["src/Modules/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "src/Modules/Catalog/Catalog.Infrastructure/"]
COPY ["src/Modules/Catalog/Catalog.Api/Catalog.Api.csproj", "src/Modules/Catalog/Catalog.Api/"]
RUN dotnet restore "Host/Host.csproj"
COPY . .
WORKDIR "/src/Host"
RUN dotnet build "./Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
COPY --from=publish /app/publish .
RUN mkdir -p /app/logs && chown -R appuser:appgroup /app/logs
USER appuser
ENTRYPOINT ["dotnet", "Host.dll"]
