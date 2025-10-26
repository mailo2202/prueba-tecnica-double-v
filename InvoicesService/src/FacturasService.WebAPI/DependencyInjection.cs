using MediatR;
using FluentValidation;
using InvoicesService.Application.Commands;
using InvoicesService.Application.Validators;

namespace InvoicesService.WebAPI;

/// <summary>
/// Application services configuration
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateInvoiceCommand).Assembly);
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(CreateInvoiceCommandValidator).Assembly);

        return services;
    }
}
