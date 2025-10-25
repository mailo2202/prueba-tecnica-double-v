using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FacturasService.Domain.Repositories;
using FacturasService.Domain.Services;
using FacturasService.Infrastructure.Data;
using FacturasService.Infrastructure.Repositories;
using FacturasService.Infrastructure.Services;

namespace FacturasService.Infrastructure;

/// <summary>
/// Configuración de dependencias de la capa de infraestructura
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuración de Entity Framework con Oracle
        services.AddDbContext<FacturasDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("OracleConnection");
            options.UseOracle(connectionString);
        });

        // Registro de repositorios
        services.AddScoped<IFacturaRepository, FacturaRepository>();

        // Configuración de HttpClient para servicios externos
        services.AddHttpClient<IClientService, ClientService>(client =>
        {
            var clientServiceUrl = configuration["Services:ClientService"];
            client.BaseAddress = new Uri(clientServiceUrl ?? "http://localhost:3001/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<IAuditService, AuditService>(client =>
        {
            var auditServiceUrl = configuration["Services:AuditService"];
            client.BaseAddress = new Uri(auditServiceUrl ?? "http://localhost:3002/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
