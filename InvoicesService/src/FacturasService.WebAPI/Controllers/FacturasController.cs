using Microsoft.AspNetCore.Mvc;
using MediatR;
using InvoicesService.Application.Commands;
using InvoicesService.Application.Queries;

namespace InvoicesService.WebAPI.Controllers;

/// <summary>
/// Controller for invoice management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(IMediator mediator, ILogger<InvoicesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new invoice
    /// </summary>
    /// <param name="request">Invoice data to create</param>
    /// <returns>Created invoice information</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateInvoiceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreateInvoiceResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand request)
    {
        try
        {
            _logger.LogInformation("Creating invoice for client {ClientId}", request.ClientId);

            var response = await _mediator.Send(request);

            if (response.Success)
            {
                return CreatedAtAction(nameof(GetInvoice), new { id = response.Id }, response);
            }

            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice");
            return StatusCode(500, new CreateInvoiceResponse
            {
                Success = false,
                Message = "Internal server error"
            });
        }
    }

    /// <summary>
    /// Gets an invoice by its ID
    /// </summary>
    /// <param name="id">Invoice ID</param>
    /// <returns>Invoice information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetInvoiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetInvoice(int id)
    {
        try
        {
            _logger.LogInformation("Getting invoice with ID {Id}", id);

            var query = new GetInvoiceByIdQuery { Id = id };
            var response = await _mediator.Send(query);

            if (response == null)
            {
                return NotFound($"Invoice with ID {id} not found");
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice with ID {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets invoices by date range
    /// </summary>
    /// <param name="startDate">Range start date</param>
    /// <param name="endDate">Range end date</param>
    /// <returns>List of invoices in the specified range</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetInvoiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetInvoicesByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date cannot be greater than end date");
            }

            _logger.LogInformation("Getting invoices from {StartDate} to {EndDate}", 
                startDate, endDate);

            var query = new GetInvoicesByDateRangeQuery
            {
                StartDate = startDate,
                EndDate = endDate
            };

            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices by date range");
            return StatusCode(500, "Internal server error");
        }
    }
}
