using MediatR;
using FluentValidation;
using FacturasService.Application.Commands;
using FacturasService.Application.Validators;

namespace FacturasService.WebAPI;

/// <summary>
/// Configuración de servicios de la aplicación
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CrearFacturaCommand).Assembly);
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(CrearFacturaCommandValidator).Assembly);

        return services;
    }
}
