using MediatR;
using FacturasService.Domain.Entities;
using FacturasService.Domain.Repositories;

namespace FacturasService.Application.Queries;

/// <summary>
/// Query para obtener una factura por ID
/// </summary>
public class ObtenerFacturaPorIdQuery : IRequest<ObtenerFacturaResponse?>
{
    public int Id { get; set; }
}

/// <summary>
/// Query para obtener facturas por rango de fechas
/// </summary>
public class ObtenerFacturasPorRangoQuery : IRequest<IEnumerable<ObtenerFacturaResponse>>
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}

/// <summary>
/// Respuesta de las queries de facturas
/// </summary>
public class ObtenerFacturaResponse
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaEmision { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string NumeroFactura { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// Handler para obtener factura por ID
/// </summary>
public class ObtenerFacturaPorIdQueryHandler : IRequestHandler<ObtenerFacturaPorIdQuery, ObtenerFacturaResponse?>
{
    private readonly IFacturaRepository _facturaRepository;

    public ObtenerFacturaPorIdQueryHandler(IFacturaRepository facturaRepository)
    {
        _facturaRepository = facturaRepository;
    }

    public async Task<ObtenerFacturaResponse?> Handle(ObtenerFacturaPorIdQuery request, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepository.ObtenerPorIdAsync(request.Id);
        
        if (factura == null)
            return null;

        return new ObtenerFacturaResponse
        {
            Id = factura.Id,
            ClientId = factura.ClientId,
            Monto = factura.Monto,
            FechaEmision = factura.FechaEmision,
            Descripcion = factura.Descripcion,
            NumeroFactura = factura.NumeroFactura,
            FechaCreacion = factura.FechaCreacion
        };
    }
}

/// <summary>
/// Handler para obtener facturas por rango de fechas
/// </summary>
public class ObtenerFacturasPorRangoQueryHandler : IRequestHandler<ObtenerFacturasPorRangoQuery, IEnumerable<ObtenerFacturaResponse>>
{
    private readonly IFacturaRepository _facturaRepository;

    public ObtenerFacturasPorRangoQueryHandler(IFacturaRepository facturaRepository)
    {
        _facturaRepository = facturaRepository;
    }

    public async Task<IEnumerable<ObtenerFacturaResponse>> Handle(ObtenerFacturasPorRangoQuery request, CancellationToken cancellationToken)
    {
        var facturas = await _facturaRepository.ObtenerPorRangoFechasAsync(request.FechaInicio, request.FechaFin);
        
        return facturas.Select(f => new ObtenerFacturaResponse
        {
            Id = f.Id,
            ClientId = f.ClientId,
            Monto = f.Monto,
            FechaEmision = f.FechaEmision,
            Descripcion = f.Descripcion,
            NumeroFactura = f.NumeroFactura,
            FechaCreacion = f.FechaCreacion
        });
    }
}
