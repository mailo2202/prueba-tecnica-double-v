using Microsoft.AspNetCore.Mvc;
using MediatR;
using FacturasService.Application.Commands;
using FluentValidation;

namespace FacturasService.WebAPI.Controllers;

/// <summary>
/// Controlador para operaciones de salud del servicio
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
    /// Verifica el estado del servicio
    /// </summary>
    /// <returns>Estado del servicio</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "FacturasService",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }
}
