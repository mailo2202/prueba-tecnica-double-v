# Use the official .NET 8.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["src/FacturasService.WebAPI/FacturasService.WebAPI.csproj", "src/FacturasService.WebAPI/"]
COPY ["src/FacturasService.Application/FacturasService.Application.csproj", "src/FacturasService.Application/"]
COPY ["src/FacturasService.Domain/FacturasService.Domain.csproj", "src/FacturasService.Domain/"]
COPY ["src/FacturasService.Infrastructure/FacturasService.Infrastructure.csproj", "src/FacturasService.Infrastructure/"]

RUN dotnet restore "src/FacturasService.WebAPI/FacturasService.WebAPI.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/FacturasService.WebAPI"
RUN dotnet build "FacturasService.WebAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "FacturasService.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "FacturasService.WebAPI.dll"]
