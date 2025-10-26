using System.ComponentModel.DataAnnotations;

namespace InvoicesService.Domain.Entities;

/// <summary>
/// Domain entity representing an Invoice
/// </summary>
public class Invoice
{
    public int Id { get; private set; }
    public int ClientId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime IssueDate { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string InvoiceNumber { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private Invoice() { }

    // Public constructor to create new invoices
    public Invoice(int clientId, decimal amount, DateTime issueDate, string description)
    {
        ClientId = clientId;
        Amount = amount;
        IssueDate = issueDate;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        InvoiceNumber = GenerateInvoiceNumber();
        
        Validate();
    }

    /// <summary>
    /// Updates the invoice data
    /// </summary>
    public void Update(decimal amount, DateTime issueDate, string description)
    {
        Amount = amount;
        IssueDate = issueDate;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        
        Validate();
    }

    /// <summary>
    /// Validates the invoice business rules
    /// </summary>
    private void Validate()
    {
        if (ClientId <= 0)
            throw new ArgumentException("Client ID must be greater than 0", nameof(ClientId));

        if (Amount <= 0)
            throw new ArgumentException("Amount must be greater than 0", nameof(Amount));

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description is required", nameof(Description));

        if (IssueDate > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException("Issue date cannot be in the future", nameof(IssueDate));
    }

    /// <summary>
    /// Generates a unique invoice number
    /// </summary>
    private string GenerateInvoiceNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"INV-{timestamp}-{random}";
    }
}
