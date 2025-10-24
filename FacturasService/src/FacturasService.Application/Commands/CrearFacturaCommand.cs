using MediatR;
using FacturasService.Domain.Entities;
using FacturasService.Domain.Repositories;
using FacturasService.Domain.Services;

namespace FacturasService.Application.Commands;

/// <summary>
/// Comando para crear una nueva factura
/// </summary>
public class CrearFacturaCommand : IRequest<CrearFacturaResponse>
{
    public int ClienteId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaEmision { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

/// <summary>
/// Respuesta del comando crear factura
/// </summary>
public class CrearFacturaResponse
{
    public int Id { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}

/// <summary>
/// Handler para el comando crear factura
/// </summary>
public class CrearFacturaCommandHandler : IRequestHandler<CrearFacturaCommand, CrearFacturaResponse>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IClienteService _clienteService;
    private readonly IAuditoriaService _auditoriaService;

    public CrearFacturaCommandHandler(
        IFacturaRepository facturaRepository,
        IClienteService clienteService,
        IAuditoriaService auditoriaService)
    {
        _facturaRepository = facturaRepository;
        _clienteService = clienteService;
        _auditoriaService = auditoriaService;
    }

    public async Task<CrearFacturaResponse> Handle(CrearFacturaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar que el cliente existe
            var clienteExiste = await _clienteService.ClienteExisteAsync(request.ClienteId);
            if (!clienteExiste)
            {
                await _auditoriaService.RegistrarEventoAsync(
                    "ERROR", 
                    "Factura", 
                    request.ClienteId, 
                    $"Cliente con ID {request.ClienteId} no existe"
                );
                
                return new CrearFacturaResponse
                {
                    Exitoso = false,
                    Mensaje = "El cliente especificado no existe"
                };
            }

            // Crear la factura
            var factura = new Factura(
                request.ClienteId,
                request.Monto,
                request.FechaEmision,
                request.Descripcion
            );

            var facturaCreada = await _facturaRepository.CrearAsync(factura);

            // Registrar evento de auditoría
            await _auditoriaService.RegistrarEventoAsync(
                "CREAR",
                "Factura",
                facturaCreada.Id,
                $"Factura creada: {facturaCreada.NumeroFactura}, Monto: {facturaCreada.Monto:C}"
            );

            return new CrearFacturaResponse
            {
                Id = facturaCreada.Id,
                NumeroFactura = facturaCreada.NumeroFactura,
                Exitoso = true,
                Mensaje = "Factura creada exitosamente"
            };
        }
        catch (Exception ex)
        {
            await _auditoriaService.RegistrarEventoAsync(
                "ERROR",
                "Factura",
                request.ClienteId,
                $"Error al crear factura: {ex.Message}"
            );

            return new CrearFacturaResponse
            {
                Exitoso = false,
                Mensaje = $"Error interno: {ex.Message}"
            };
        }
    }
}
