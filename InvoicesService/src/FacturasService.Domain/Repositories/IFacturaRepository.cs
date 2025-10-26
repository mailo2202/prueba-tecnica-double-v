using InvoicesService.Domain.Entities;

namespace InvoicesService.Domain.Repositories;

/// <summary>
/// Invoice repository interface
/// </summary>
public interface IInvoiceRepository
{
    /// <summary>
    /// Gets an invoice by its ID
    /// </summary>
    Task<Invoice?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all invoices within a date range
    /// </summary>
    Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets all invoices for a specific client
    /// </summary>
    Task<IEnumerable<Invoice>> GetByClientAsync(int clientId);

    /// <summary>
    /// Creates a new invoice
    /// </summary>
    Task<Invoice> CreateAsync(Invoice invoice);

    /// <summary>
    /// Updates an existing invoice
    /// </summary>
    Task<Invoice> UpdateAsync(Invoice invoice);

    /// <summary>
    /// Checks if an invoice exists with the specified ID
    /// </summary>
    Task<bool> ExistsAsync(int id);
}
