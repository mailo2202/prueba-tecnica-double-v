using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using InvoicesService.Domain.Repositories;
using InvoicesService.Domain.Services;
using InvoicesService.Infrastructure.Data;
using InvoicesService.Infrastructure.Repositories;
using InvoicesService.Infrastructure.Services;

namespace InvoicesService.Infrastructure;

/// <summary>
/// Infrastructure layer dependency configuration
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Entity Framework configuration with Oracle
        services.AddDbContext<InvoicesDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("OracleConnection");
            options.UseOracle(connectionString);
        });

        // Repository registration
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        // HttpClient configuration for external services
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
