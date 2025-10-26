using Microsoft.EntityFrameworkCore;
using InvoicesService.Domain.Entities;
using InvoicesService.Domain.Repositories;
using InvoicesService.Infrastructure.Data;

namespace InvoicesService.Infrastructure.Repositories;

/// <summary>
/// Invoice repository implementation using Entity Framework
/// </summary>
public class InvoiceRepository : IInvoiceRepository
{
    private readonly InvoicesDbContext _context;

    public InvoiceRepository(InvoicesDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _context.Invoices
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Invoices
            .Where(f => f.IssueDate >= startDate && f.IssueDate <= endDate)
            .OrderByDescending(f => f.IssueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByClientAsync(int clientId)
    {
        return await _context.Invoices
            .Where(f => f.ClientId == clientId)
            .OrderByDescending(f => f.IssueDate)
            .ToListAsync();
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task<Invoice> UpdateAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Invoices
            .AnyAsync(f => f.Id == id);
    }
}
