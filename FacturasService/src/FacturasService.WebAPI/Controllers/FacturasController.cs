using Microsoft.AspNetCore.Mvc;
using MediatR;
using FacturasService.Application.Commands;
using FacturasService.Application.Queries;

namespace FacturasService.WebAPI.Controllers;

/// <summary>
/// Controlador para la gestión de facturas
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FacturasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FacturasController> _logger;

    public FacturasController(IMediator mediator, ILogger<FacturasController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Crea una nueva factura
    /// </summary>
    /// <param name="request">Datos de la factura a crear</param>
    /// <returns>Información de la factura creada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CrearFacturaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CrearFacturaResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CrearFactura([FromBody] CrearFacturaCommand request)
    {
        try
        {
            _logger.LogInformation("Creando factura para client {ClientId}", request.ClientId);

            var response = await _mediator.Send(request);

            if (response.Exitoso)
            {
                return CreatedAtAction(nameof(ObtenerFactura), new { id = response.Id }, response);
            }

            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear factura");
            return StatusCode(500, new CrearFacturaResponse
            {
                Exitoso = false,
                Mensaje = "Error interno del servidor"
            });
        }
    }

    /// <summary>
    /// Obtiene una factura por su ID
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <returns>Información de la factura</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ObtenerFacturaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerFactura(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo factura con ID {Id}", id);

            var query = new ObtenerFacturaPorIdQuery { Id = id };
            var response = await _mediator.Send(query);

            if (response == null)
            {
                return NotFound($"Factura con ID {id} no encontrada");
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener factura con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene facturas por rango de fechas
    /// </summary>
    /// <param name="fechaInicio">Fecha de inicio del rango</param>
    /// <param name="fechaFin">Fecha de fin del rango</param>
    /// <returns>Lista de facturas en el rango especificado</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ObtenerFacturaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerFacturasPorRango(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        try
        {
            if (fechaInicio > fechaFin)
            {
                return BadRequest("La fecha de inicio no puede ser mayor a la fecha de fin");
            }

            _logger.LogInformation("Obteniendo facturas desde {FechaInicio} hasta {FechaFin}", 
                fechaInicio, fechaFin);

            var query = new ObtenerFacturasPorRangoQuery
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener facturas por rango");
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
