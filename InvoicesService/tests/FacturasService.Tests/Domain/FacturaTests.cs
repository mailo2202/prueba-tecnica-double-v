using InvoicesService.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace InvoicesService.Tests.Domain;

/// <summary>
/// Unit tests for the Invoice entity
/// </summary>
public class InvoiceTests
{
    [Fact]
    public void CreateInvoice_WithValidData_ShouldCreateInvoiceCorrectly()
    {
        // Arrange
        var clientId = 1;
        var amount = 150000.50m;
        var issueDate = DateTime.UtcNow;
        var description = "Consulting services";

        // Act
        var invoice = new Invoice(clientId, amount, issueDate, description);

        // Assert
        invoice.ClientId.Should().Be(clientId);
        invoice.Amount.Should().Be(amount);
        invoice.IssueDate.Should().Be(issueDate);
        invoice.Description.Should().Be(description);
        invoice.InvoiceNumber.Should().NotBeNullOrEmpty();
        invoice.InvoiceNumber.Should().StartWith("INV-");
        invoice.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateInvoice_WithInvalidClientId_ShouldThrowException(int invalidClientId)
    {
        // Arrange
        var amount = 150000.50m;
        var issueDate = DateTime.UtcNow;
        var description = "Consulting services";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Invoice(invalidClientId, amount, issueDate, description));
        
        exception.Message.Should().Contain("Client ID must be greater than 0");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void CreateInvoice_WithInvalidAmount_ShouldThrowException(decimal invalidAmount)
    {
        // Arrange
        var clientId = 1;
        var issueDate = DateTime.UtcNow;
        var description = "Consulting services";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Invoice(clientId, invalidAmount, issueDate, description));
        
        exception.Message.Should().Contain("Amount must be greater than 0");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateInvoice_WithInvalidDescription_ShouldThrowException(string invalidDescription)
    {
        // Arrange
        var clientId = 1;
        var amount = 150000.50m;
        var issueDate = DateTime.UtcNow;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Invoice(clientId, amount, issueDate, invalidDescription));
        
        exception.Message.Should().Contain("Description is required");
    }

    [Fact]
    public void CreateInvoice_WithFutureIssueDate_ShouldThrowException()
    {
        // Arrange
        var clientId = 1;
        var amount = 150000.50m;
        var futureIssueDate = DateTime.UtcNow.AddDays(2);
        var description = "Consulting services";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Invoice(clientId, amount, futureIssueDate, description));
        
        exception.Message.Should().Contain("Issue date cannot be in the future");
    }

    [Fact]
    public void UpdateInvoice_WithValidData_ShouldUpdateCorrectly()
    {
        // Arrange
        var invoice = new Invoice(1, 100000, DateTime.UtcNow, "Initial description");
        var newAmount = 200000;
        var newIssueDate = DateTime.UtcNow.AddDays(-1);
        var newDescription = "Updated description";

        // Act
        invoice.Update(newAmount, newIssueDate, newDescription);

        // Assert
        invoice.Amount.Should().Be(newAmount);
        invoice.IssueDate.Should().Be(newIssueDate);
        invoice.Description.Should().Be(newDescription);
        invoice.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void GenerateInvoiceNumber_ShouldGenerateUniqueNumber()
    {
        // Arrange & Act
        var invoice1 = new Invoice(1, 100000, DateTime.UtcNow, "Invoice 1");
        var invoice2 = new Invoice(2, 200000, DateTime.UtcNow, "Invoice 2");

        // Assert
        invoice1.InvoiceNumber.Should().NotBe(invoice2.InvoiceNumber);
        invoice1.InvoiceNumber.Should().StartWith("INV-");
        invoice2.InvoiceNumber.Should().StartWith("INV-");
    }
}
