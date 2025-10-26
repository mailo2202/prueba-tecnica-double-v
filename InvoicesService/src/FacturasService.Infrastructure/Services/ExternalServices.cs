using Microsoft.Extensions.Logging;
using InvoicesService.Domain.Services;

namespace InvoicesService.Infrastructure.Services;

/// <summary>
/// Service to validate client via HTTP
/// </summary>
public class ClientService : IClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ClientService> _logger;

    public ClientService(HttpClient httpClient, ILogger<ClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> ClientExistsAsync(int clientId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"client/{clientId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating client with ID {ClientId}", clientId);
            return false;
        }
    }
}

/// <summary>
/// Service to register audit events
/// </summary>
public class AuditService : IAuditService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuditService> _logger;

    public AuditService(HttpClient httpClient, ILogger<AuditService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task RegisterEventAsync(string eventType, string entity, int entityId, string details)
    {
        try
        {
            var auditEvent = new
            {
                event_type = eventType,
                entity = entity,
                entity_id = entityId,
                details = details,
                timestamp = DateTime.UtcNow,
                service = "InvoicesService"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(auditEvent);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            await _httpClient.PostAsync("api/v1/audit", content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering audit event: {EventType} - {Entity} - {EntityId}", 
                eventType, entity, entityId);
        }
    }
}
