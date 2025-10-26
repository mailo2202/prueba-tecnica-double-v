using InvoicesService.Application.Commands;
using InvoicesService.Domain.Repositories;
using InvoicesService.Domain.Services;
using Moq;
using FluentAssertions;
using Xunit;

namespace InvoicesService.Tests.Application;

/// <summary>
/// Unit tests for the CreateInvoiceCommand
/// </summary>
public class CreateInvoiceCommandHandlerTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IClientService> _clientServiceMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly CreateInvoiceCommandHandler _handler;

    public CreateInvoiceCommandHandlerTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _clientServiceMock = new Mock<IClientService>();
        _auditServiceMock = new Mock<IAuditService>();
        
        _handler = new CreateInvoiceCommandHandler(
            _invoiceRepositoryMock.Object,
            _clientServiceMock.Object,
            _auditServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithExistingClient_ShouldCreateInvoiceSuccessfully()
    {
        // Arrange
        var command = new CreateInvoiceCommand
        {
            ClientId = 1,
            Amount = 150000,
            IssueDate = DateTime.UtcNow,
            Description = "Consulting services"
        };

        _clientServiceMock.Setup(x => x.ClientExistsAsync(command.ClientId))
            .ReturnsAsync(true);

        _invoiceRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Invoice>()))
            .ReturnsAsync((Domain.Entities.Invoice f) => f);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Invoice created successfully");
        result.Id.Should().BeGreaterThan(0);
        result.InvoiceNumber.Should().NotBeNullOrEmpty();

        _clientServiceMock.Verify(x => x.ClientExistsAsync(command.ClientId), Times.Once);
        _invoiceRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Domain.Entities.Invoice>()), Times.Once);
        _auditServiceMock.Verify(x => x.RegisterEventAsync(
            "CREATE", "Invoice", It.IsAny<int>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentClient_ShouldReturnError()
    {
        // Arrange
        var command = new CreateInvoiceCommand
        {
            ClientId = 999,
            Amount = 150000,
            IssueDate = DateTime.UtcNow,
            Description = "Consulting services"
        };

        _clientServiceMock.Setup(x => x.ClientExistsAsync(command.ClientId))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("The specified client does not exist");

        _clientServiceMock.Verify(x => x.ClientExistsAsync(command.ClientId), Times.Once);
        _invoiceRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Domain.Entities.Invoice>()), Times.Never);
        _auditServiceMock.Verify(x => x.RegisterEventAsync(
            "ERROR", "Invoice", command.ClientId, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithRepositoryException_ShouldReturnError()
    {
        // Arrange
        var command = new CreateInvoiceCommand
        {
            ClientId = 1,
            Amount = 150000,
            IssueDate = DateTime.UtcNow,
            Description = "Consulting services"
        };

        _clientServiceMock.Setup(x => x.ClientExistsAsync(command.ClientId))
            .ReturnsAsync(true);

        _invoiceRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Invoice>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Internal error");

        _auditServiceMock.Verify(x => x.RegisterEventAsync(
            "ERROR", "Invoice", command.ClientId, It.IsAny<string>()), Times.Once);
    }
}
