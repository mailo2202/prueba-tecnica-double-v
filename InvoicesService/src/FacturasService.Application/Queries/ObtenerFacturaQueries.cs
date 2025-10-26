using MediatR;
using InvoicesService.Domain.Entities;
using InvoicesService.Domain.Repositories;

namespace InvoicesService.Application.Queries;

/// <summary>
/// Query to get an invoice by ID
/// </summary>
public class GetInvoiceByIdQuery : IRequest<GetInvoiceResponse?>
{
    public int Id { get; set; }
}

/// <summary>
/// Query to get invoices by date range
/// </summary>
public class GetInvoicesByDateRangeQuery : IRequest<IEnumerable<GetInvoiceResponse>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

/// <summary>
/// Response to invoice queries
/// </summary>
public class GetInvoiceResponse
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssueDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Handler to get invoice by ID
/// </summary>
public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, GetInvoiceResponse?>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<GetInvoiceResponse?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id);
        
        if (invoice == null)
            return null;

        return new GetInvoiceResponse
        {
            Id = invoice.Id,
            ClientId = invoice.ClientId,
            Amount = invoice.Amount,
            IssueDate = invoice.IssueDate,
            Description = invoice.Description,
            InvoiceNumber = invoice.InvoiceNumber,
            CreatedAt = invoice.CreatedAt
        };
    }
}

/// <summary>
/// Handler to get invoices by date range
/// </summary>
public class GetInvoicesByDateRangeQueryHandler : IRequestHandler<GetInvoicesByDateRangeQuery, IEnumerable<GetInvoiceResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoicesByDateRangeQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<IEnumerable<GetInvoiceResponse>> Handle(GetInvoicesByDateRangeQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetByDateRangeAsync(request.StartDate, request.EndDate);
        
        return invoices.Select(f => new GetInvoiceResponse
        {
            Id = f.Id,
            ClientId = f.ClientId,
            Amount = f.Amount,
            IssueDate = f.IssueDate,
            Description = f.Description,
            InvoiceNumber = f.InvoiceNumber,
            CreatedAt = f.CreatedAt
        });
    }
}
