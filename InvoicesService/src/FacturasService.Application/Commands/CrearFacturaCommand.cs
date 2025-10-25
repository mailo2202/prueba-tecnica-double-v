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
    public int ClientId { get; set; }
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
    private readonly IClientService _clientService;
    private readonly IAuditService _auditService;

    public CrearFacturaCommandHandler(
        IFacturaRepository facturaRepository,
        IClientService clientService,
        IAuditService auditService)
    {
        _facturaRepository = facturaRepository;
        _clientService = clientService;
        _auditService = auditService;
    }

    public async Task<CrearFacturaResponse> Handle(CrearFacturaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar que el cliente existe
            var clientExiste = await _clientService.ClientExisteAsync(request.ClientId);
            if (!clientExiste)
            {
                await _auditService.RegistrarEventoAsync(
                    "ERROR", 
                    "Factura", 
                    request.ClientId, 
                    $"Client con ID {request.ClientId} no existe"
                );
                
                return new CrearFacturaResponse
                {
                    Exitoso = false,
                    Mensaje = "El client especificado no existe"
                };
            }

            // Crear la factura
            var factura = new Factura(
                request.ClientId,
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
                request.ClientId,
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
