using Microsoft.Extensions.Logging;
using FacturasService.Domain.Services;

namespace FacturasService.Infrastructure.Services;

/// <summary>
/// Servicio para validar client mediante HTTP
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

    public async Task<bool> ClientExisteAsync(int clientId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"client/{clientId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar client con ID {ClientId}", clientId);
            return false;
        }
    }
}

/// <summary>
/// Servicio para registrar eventos de auditoría
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

    public async Task RegistrarEventoAsync(string evento, string entidad, int entidadId, string detalles)
    {
        try
        {
            var eventoAuditoria = new
            {
                event_type = evento,
                entity = entidad,
                entity_id = entidadId,
                details = detalles,
                timestamp = DateTime.UtcNow,
                service = "FacturasService"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(eventoAuditoria);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            await _httpClient.PostAsync("api/v1/audit", content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar evento de auditoría: {Evento} - {Entidad} - {EntidadId}", 
                evento, entidad, entidadId);
        }
    }
}
