using FluentValidation;

namespace FacturasService.Application.Validators;

/// <summary>
/// Validador para el comando crear factura
/// </summary>
public class CrearFacturaCommandValidator : AbstractValidator<Commands.CrearFacturaCommand>
{
    public CrearFacturaCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .GreaterThan(0)
            .WithMessage("El ID del client debe ser mayor a 0");

        RuleFor(x => x.Monto)
            .GreaterThan(0)
            .WithMessage("El monto debe ser mayor a 0");

        RuleFor(x => x.Descripcion)
            .NotEmpty()
            .WithMessage("La descripción es requerida")
            .MaximumLength(500)
            .WithMessage("La descripción no puede exceder 500 caracteres");

        RuleFor(x => x.FechaEmision)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("La fecha de emisión no puede ser futura");
    }
}
