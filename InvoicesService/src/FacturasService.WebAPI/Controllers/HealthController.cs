using Microsoft.AspNetCore.Mvc;

namespace InvoicesService.WebAPI.Controllers;

/// <summary>
/// Controller for service health operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Verifies the service status
    /// </summary>
    /// <returns>Service status</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "InvoicesService",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }
}
