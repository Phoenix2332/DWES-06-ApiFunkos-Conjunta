using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Mappers;
using ApiFunkosConjunta.Models;
using FluentAssertions;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Tests.Mappers.Proveedores;

[TestFixture]
public class ProveedorMapperCasosCorrectosTest {
    [Test]
    public void ToModel_DatosValidos_RetornaProveedor() {
        // Arrange
        var request = new CrearProveedorDto {
            Nombre = "  Logitech  ",
            Contacto = "  exportaciones@logitech.com  ",
            Pais = "  Suiza  "
        };

        // Act
        var result = ProveedorMapper.ToModel(request);

        // Assert
        result.Should().NotBeNull();
        result.Nombre.Should().Be("Logitech");
        result.Contacto.Should().Be("exportaciones@logitech.com");
        result.Pais.Should().Be("Suiza");
    }

    [Test]
    public void ToDto_ProveedorValido_RetornaProveedorDto() {
        // Arrange
        var proveedor = new Proveedor {
            Id = ObjectId.Parse("64b0000000000000000000ff"),
            Nombre = "Logitech",
            Contacto = "exportaciones@logitech.com",
            Pais = "Suiza"
        };

        // Act
        var result = ProveedorMapper.ToDto(proveedor);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(proveedor.Id);
        result.Nombre.Should().Be(proveedor.Nombre);
        result.Contacto.Should().Be(proveedor.Contacto);
        result.Pais.Should().Be(proveedor.Pais);
    }

    [Test]
    public void UpdateModel_DatosValidos_ActualizaProveedor() {
        // Arrange
        var proveedor = new Proveedor {
            Id = ObjectId.Parse("64b0000000000000000000ff"),
            Nombre = "Logitech",
            Contacto = "exportaciones@logitech.com",
            Pais = "Suiza"
        };

        var request = new ActualizarProveedorDto {
            Nombre = "  Funko  ",
            Contacto = "  ventas@funko.com  ",
            Pais = "  Estados Unidos  "
        };

        // Act
        ProveedorMapper.UpdateModel(proveedor, request);

        // Assert
        proveedor.Nombre.Should().Be("Funko");
        proveedor.Contacto.Should().Be("ventas@funko.com");
        proveedor.Pais.Should().Be("Estados Unidos");
        proveedor.Id.Should().Be(ObjectId.Parse("64b0000000000000000000ff"));
    }
}
