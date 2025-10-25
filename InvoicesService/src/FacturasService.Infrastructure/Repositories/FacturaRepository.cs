using Microsoft.EntityFrameworkCore;
using FacturasService.Domain.Entities;
using FacturasService.Domain.Repositories;
using FacturasService.Infrastructure.Data;

namespace FacturasService.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de facturas usando Entity Framework
/// </summary>
public class FacturaRepository : IFacturaRepository
{
    private readonly FacturasDbContext _context;

    public FacturaRepository(FacturasDbContext context)
    {
        _context = context;
    }

    public async Task<Factura?> ObtenerPorIdAsync(int id)
    {
        return await _context.Facturas
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Factura>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _context.Facturas
            .Where(f => f.FechaEmision >= fechaInicio && f.FechaEmision <= fechaFin)
            .OrderByDescending(f => f.FechaEmision)
            .ToListAsync();
    }

    public async Task<IEnumerable<Factura>> ObtenerPorClientAsync(int clientId)
    {
        return await _context.Facturas
            .Where(f => f.ClientId == clientId)
            .OrderByDescending(f => f.FechaEmision)
            .ToListAsync();
    }

    public async Task<Factura> CrearAsync(Factura factura)
    {
        _context.Facturas.Add(factura);
        await _context.SaveChangesAsync();
        return factura;
    }

    public async Task<Factura> ActualizarAsync(Factura factura)
    {
        _context.Facturas.Update(factura);
        await _context.SaveChangesAsync();
        return factura;
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Facturas
            .AnyAsync(f => f.Id == id);
    }
}
