using System.ComponentModel.DataAnnotations;

namespace FacturasService.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa una Factura
/// </summary>
public class Factura
{
    public int Id { get; private set; }
    public int ClientId { get; private set; }
    public decimal Monto { get; private set; }
    public DateTime FechaEmision { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public string NumeroFactura { get; private set; } = string.Empty;
    public DateTime FechaCreacion { get; private set; }
    public DateTime? FechaActualizacion { get; private set; }

    // Constructor privado para EF Core
    private Factura() { }

    // Constructor público para crear nuevas facturas
    public Factura(int clientId, decimal monto, DateTime fechaEmision, string descripcion)
    {
        ClientId = clientId;
        Monto = monto;
        FechaEmision = fechaEmision;
        Descripcion = descripcion;
        FechaCreacion = DateTime.UtcNow;
        NumeroFactura = GenerarNumeroFactura();
        
        Validar();
    }

    /// <summary>
    /// Actualiza los datos de la factura
    /// </summary>
    public void Actualizar(decimal monto, DateTime fechaEmision, string descripcion)
    {
        Monto = monto;
        FechaEmision = fechaEmision;
        Descripcion = descripcion;
        FechaActualizacion = DateTime.UtcNow;
        
        Validar();
    }

    /// <summary>
    /// Valida las reglas de negocio de la factura
    /// </summary>
    private void Validar()
    {
        if (ClientId <= 0)
            throw new ArgumentException("El ID del client debe ser mayor a 0", nameof(ClientId));

        if (Monto <= 0)
            throw new ArgumentException("El monto debe ser mayor a 0", nameof(Monto));

        if (string.IsNullOrWhiteSpace(Descripcion))
            throw new ArgumentException("La descripción es requerida", nameof(Descripcion));

        if (FechaEmision > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException("La fecha de emisión no puede ser futura", nameof(FechaEmision));
    }

    /// <summary>
    /// Genera un número de factura único
    /// </summary>
    private string GenerarNumeroFactura()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"FAC-{timestamp}-{random}";
    }
}
