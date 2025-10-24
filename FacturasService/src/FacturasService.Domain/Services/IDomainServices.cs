namespace FacturasService.Domain.Services;

/// <summary>
/// Servicio de dominio para validaciones de facturas
/// </summary>
public interface IClienteService
{
    /// <summary>
    /// Valida si un cliente existe en el sistema
    /// </summary>
    Task<bool> ClienteExisteAsync(int clienteId);
}

/// <summary>
/// Servicio de dominio para auditoría
/// </summary>
public interface IAuditoriaService
{
    /// <summary>
    /// Registra un evento de auditoría
    /// </summary>
    Task RegistrarEventoAsync(string evento, string entidad, int entidadId, string detalles);
}
