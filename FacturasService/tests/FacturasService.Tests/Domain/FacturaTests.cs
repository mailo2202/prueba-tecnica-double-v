using FacturasService.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace FacturasService.Tests.Domain;

/// <summary>
/// Pruebas unitarias para la entidad Factura
/// </summary>
public class FacturaTests
{
    [Fact]
    public void CrearFactura_ConDatosValidos_DebeCrearFacturaCorrectamente()
    {
        // Arrange
        var clientId = 1;
        var monto = 150000.50m;
        var fechaEmision = DateTime.UtcNow;
        var descripcion = "Servicios de consultoría";

        // Act
        var factura = new Factura(clientId, monto, fechaEmision, descripcion);

        // Assert
        factura.ClientId.Should().Be(clientId);
        factura.Monto.Should().Be(monto);
        factura.FechaEmision.Should().Be(fechaEmision);
        factura.Descripcion.Should().Be(descripcion);
        factura.NumeroFactura.Should().NotBeNullOrEmpty();
        factura.NumeroFactura.Should().StartWith("FAC-");
        factura.FechaCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CrearFactura_ConClientIdInvalido_DebeLanzarExcepcion(int clientIdInvalido)
    {
        // Arrange
        var monto = 150000.50m;
        var fechaEmision = DateTime.UtcNow;
        var descripcion = "Servicios de consultoría";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Factura(clientIdInvalido, monto, fechaEmision, descripcion));
        
        exception.Message.Should().Contain("El ID del client debe ser mayor a 0");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void CrearFactura_ConMontoInvalido_DebeLanzarExcepcion(decimal montoInvalido)
    {
        // Arrange
        var clientId = 1;
        var fechaEmision = DateTime.UtcNow;
        var descripcion = "Servicios de consultoría";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Factura(clientId, montoInvalido, fechaEmision, descripcion));
        
        exception.Message.Should().Contain("El monto debe ser mayor a 0");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CrearFactura_ConDescripcionInvalida_DebeLanzarExcepcion(string descripcionInvalida)
    {
        // Arrange
        var clientId = 1;
        var monto = 150000.50m;
        var fechaEmision = DateTime.UtcNow;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Factura(clientId, monto, fechaEmision, descripcionInvalida));
        
        exception.Message.Should().Contain("La descripción es requerida");
    }

    [Fact]
    public void CrearFactura_ConFechaEmisionFutura_DebeLanzarExcepcion()
    {
        // Arrange
        var clientId = 1;
        var monto = 150000.50m;
        var fechaEmisionFutura = DateTime.UtcNow.AddDays(2);
        var descripcion = "Servicios de consultoría";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Factura(clientId, monto, fechaEmisionFutura, descripcion));
        
        exception.Message.Should().Contain("La fecha de emisión no puede ser futura");
    }

    [Fact]
    public void ActualizarFactura_ConDatosValidos_DebeActualizarCorrectamente()
    {
        // Arrange
        var factura = new Factura(1, 100000, DateTime.UtcNow, "Descripción inicial");
        var nuevoMonto = 200000;
        var nuevaFechaEmision = DateTime.UtcNow.AddDays(-1);
        var nuevaDescripcion = "Descripción actualizada";

        // Act
        factura.Actualizar(nuevoMonto, nuevaFechaEmision, nuevaDescripcion);

        // Assert
        factura.Monto.Should().Be(nuevoMonto);
        factura.FechaEmision.Should().Be(nuevaFechaEmision);
        factura.Descripcion.Should().Be(nuevaDescripcion);
        factura.FechaActualizacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void GenerarNumeroFactura_DebeGenerarNumeroUnico()
    {
        // Arrange & Act
        var factura1 = new Factura(1, 100000, DateTime.UtcNow, "Factura 1");
        var factura2 = new Factura(2, 200000, DateTime.UtcNow, "Factura 2");

        // Assert
        factura1.NumeroFactura.Should().NotBe(factura2.NumeroFactura);
        factura1.NumeroFactura.Should().StartWith("FAC-");
        factura2.NumeroFactura.Should().StartWith("FAC-");
    }
}
