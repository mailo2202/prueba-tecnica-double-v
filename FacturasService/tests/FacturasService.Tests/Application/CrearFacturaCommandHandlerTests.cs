using FacturasService.Application.Commands;
using FacturasService.Domain.Repositories;
using FacturasService.Domain.Services;
using Moq;
using FluentAssertions;
using Xunit;

namespace FacturasService.Tests.Application;

/// <summary>
/// Pruebas unitarias para el comando CrearFacturaCommand
/// </summary>
public class CrearFacturaCommandHandlerTests
{
    private readonly Mock<IFacturaRepository> _facturaRepositoryMock;
    private readonly Mock<IClientService> _clientServiceMock;
    private readonly Mock<IAuditoriaService> _auditoriaServiceMock;
    private readonly CrearFacturaCommandHandler _handler;

    public CrearFacturaCommandHandlerTests()
    {
        _facturaRepositoryMock = new Mock<IFacturaRepository>();
        _clientServiceMock = new Mock<IClientService>();
        _auditoriaServiceMock = new Mock<IAuditoriaService>();
        
        _handler = new CrearFacturaCommandHandler(
            _facturaRepositoryMock.Object,
            _clientServiceMock.Object,
            _auditoriaServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ConClientExistente_DebeCrearFacturaExitosamente()
    {
        // Arrange
        var command = new CrearFacturaCommand
        {
            ClientId = 1,
            Monto = 150000,
            FechaEmision = DateTime.UtcNow,
            Descripcion = "Servicios de consultoría"
        };

        _clientServiceMock.Setup(x => x.ClientExisteAsync(command.ClientId))
            .ReturnsAsync(true);

        _facturaRepositoryMock.Setup(x => x.CrearAsync(It.IsAny<Domain.Entities.Factura>()))
            .ReturnsAsync((Domain.Entities.Factura f) => f);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Exitoso.Should().BeTrue();
        result.Mensaje.Should().Be("Factura creada exitosamente");
        result.Id.Should().BeGreaterThan(0);
        result.NumeroFactura.Should().NotBeNullOrEmpty();

        _clientServiceMock.Verify(x => x.ClientExisteAsync(command.ClientId), Times.Once);
        _facturaRepositoryMock.Verify(x => x.CrearAsync(It.IsAny<Domain.Entities.Factura>()), Times.Once);
        _auditoriaServiceMock.Verify(x => x.RegistrarEventoAsync(
            "CREAR", "Factura", It.IsAny<int>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConClientInexistente_DebeRetornarError()
    {
        // Arrange
        var command = new CrearFacturaCommand
        {
            ClientId = 999,
            Monto = 150000,
            FechaEmision = DateTime.UtcNow,
            Descripcion = "Servicios de consultoría"
        };

        _clientServiceMock.Setup(x => x.ClientExisteAsync(command.ClientId))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Exitoso.Should().BeFalse();
        result.Mensaje.Should().Be("El client especificado no existe");

        _clientServiceMock.Verify(x => x.ClientExisteAsync(command.ClientId), Times.Once);
        _facturaRepositoryMock.Verify(x => x.CrearAsync(It.IsAny<Domain.Entities.Factura>()), Times.Never);
        _auditoriaServiceMock.Verify(x => x.RegistrarEventoAsync(
            "ERROR", "Factura", command.ClientId, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ConExcepcionEnRepositorio_DebeRetornarError()
    {
        // Arrange
        var command = new CrearFacturaCommand
        {
            ClientId = 1,
            Monto = 150000,
            FechaEmision = DateTime.UtcNow,
            Descripcion = "Servicios de consultoría"
        };

        _clientServiceMock.Setup(x => x.ClientExisteAsync(command.ClientId))
            .ReturnsAsync(true);

        _facturaRepositoryMock.Setup(x => x.CrearAsync(It.IsAny<Domain.Entities.Factura>()))
            .ThrowsAsync(new Exception("Error de base de datos"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Exitoso.Should().BeFalse();
        result.Mensaje.Should().Contain("Error interno");

        _auditoriaServiceMock.Verify(x => x.RegistrarEventoAsync(
            "ERROR", "Factura", command.ClientId, It.IsAny<string>()), Times.Once);
    }
}
