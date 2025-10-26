namespace InvoicesService.Domain.Services;

/// <summary>
/// Domain service for invoice validations
/// </summary>
public interface IClientService
{
    /// <summary>
    /// Validates if a client exists in the system
    /// </summary>
    Task<bool> ClientExistsAsync(int clientId);
}

/// <summary>
/// Domain service for auditing
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Registers an audit event
    /// </summary>
    Task RegisterEventAsync(string eventType, string entity, int entityId, string details);
}
