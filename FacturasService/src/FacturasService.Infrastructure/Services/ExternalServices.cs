using FacturasService.Domain.Services;

namespace FacturasService.Infrastructure.Services;

/// <summary>
/// Servicio para validar clientes mediante HTTP
/// </summary>
public class ClienteService : IClienteService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(HttpClient httpClient, ILogger<ClienteService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> ClienteExisteAsync(int clienteId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"clientes/{clienteId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar cliente con ID {ClienteId}", clienteId);
            return false;
        }
    }
}

/// <summary>
/// Servicio para registrar eventos de auditoría
/// </summary>
public class AuditoriaService : IAuditoriaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuditoriaService> _logger;

    public AuditoriaService(HttpClient httpClient, ILogger<AuditoriaService> logger)
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
