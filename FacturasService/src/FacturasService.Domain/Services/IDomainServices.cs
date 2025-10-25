namespace FacturasService.Domain.Services;

/// <summary>
/// Servicio de dominio para validaciones de facturas
/// </summary>
public interface IClientService
{
    /// <summary>
    /// Valida si un client existe en el sistema
    /// </summary>
    Task<bool> ClientExisteAsync(int clientId);
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
