using MediatR;
using InvoicesService.Domain.Entities;
using InvoicesService.Domain.Repositories;
using InvoicesService.Domain.Services;

namespace InvoicesService.Application.Commands;

/// <summary>
/// Command to create a new invoice
/// </summary>
public class CreateInvoiceCommand : IRequest<CreateInvoiceResponse>
{
    public int ClientId { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssueDate { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Response to the create invoice command
/// </summary>
public class CreateInvoiceResponse
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Handler for the create invoice command
/// </summary>
public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, CreateInvoiceResponse>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IClientService _clientService;
    private readonly IAuditService _auditService;

    public CreateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IClientService clientService,
        IAuditService auditService)
    {
        _invoiceRepository = invoiceRepository;
        _clientService = clientService;
        _auditService = auditService;
    }

    public async Task<CreateInvoiceResponse> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate that the client exists
            var clientExists = await _clientService.ClientExistsAsync(request.ClientId);
            if (!clientExists)
            {
                await _auditService.RegisterEventAsync(
                    "ERROR", 
                    "Invoice", 
                    request.ClientId, 
                    $"Client with ID {request.ClientId} does not exist"
                );
                
                return new CreateInvoiceResponse
                {
                    Success = false,
                    Message = "The specified client does not exist"
                };
            }

            // Create the invoice
            var invoice = new Invoice(
                request.ClientId,
                request.Amount,
                request.IssueDate,
                request.Description
            );

            var createdInvoice = await _invoiceRepository.CreateAsync(invoice);

            // Register audit event
            await _auditService.RegisterEventAsync(
                "CREATE",
                "Invoice",
                createdInvoice.Id,
                $"Invoice created: {createdInvoice.InvoiceNumber}, Amount: {createdInvoice.Amount:C}"
            );

            return new CreateInvoiceResponse
            {
                Id = createdInvoice.Id,
                InvoiceNumber = createdInvoice.InvoiceNumber,
                Success = true,
                Message = "Invoice created successfully"
            };
        }
        catch (Exception ex)
        {
            await _auditService.RegisterEventAsync(
                "ERROR",
                "Invoice",
                request.ClientId,
                $"Error creating invoice: {ex.Message}"
            );

            return new CreateInvoiceResponse
            {
                Success = false,
                Message = $"Internal error: {ex.Message}"
            };
        }
    }
}
