namespace FacturasService.Domain.Repositories;

/// <summary>
/// Interfaz del repositorio para la entidad Factura
/// </summary>
public interface IFacturaRepository
{
    /// <summary>
    /// Obtiene una factura por su ID
    /// </summary>
    Task<Factura?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Obtiene todas las facturas en un rango de fechas
    /// </summary>
    Task<IEnumerable<Factura>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene todas las facturas de un cliente
    /// </summary>
    Task<IEnumerable<Factura>> ObtenerPorClienteAsync(int clienteId);

    /// <summary>
    /// Crea una nueva factura
    /// </summary>
    Task<Factura> CrearAsync(Factura factura);

    /// <summary>
    /// Actualiza una factura existente
    /// </summary>
    Task<Factura> ActualizarAsync(Factura factura);

    /// <summary>
    /// Verifica si existe una factura con el ID especificado
    /// </summary>
    Task<bool> ExisteAsync(int id);
}
