using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Proveedores;

[TestFixture]
public class ProveedorServiceCasosCorrectosTest : ProveedorServiceTestBase {
    [Test]
    public async Task GetAllAsync_ConProveedores_RetornaProveedoresMapeados() {
        // Arrange
        var proveedores = new List<Proveedor> {
            CreateProveedor(),
            CreateProveedor("64b0000000000000000000fe", "Ikea")
        };

        Repository
            .Setup(r => r.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Success<(IReadOnlyList<Proveedor> Proveedores, int Total), DomainErrors>((proveedores, 2)));

        // Act
        var result = await Service.GetAllAsync(null, 1, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Proveedores.Should().HaveCount(2);
        result.Value.Total.Should().Be(2);
        result.Value.Proveedores[0].Id.Should().Be(proveedores[0].Id);
        result.Value.Proveedores[0].Nombre.Should().Be("Logitech");
        result.Value.Proveedores[0].Contacto.Should().Be("exportaciones@logitech.com");
        result.Value.Proveedores[1].Nombre.Should().Be("Ikea");
    }

    [Test]
    public async Task GetAllAsync_ConFiltroYpaginacion_ConsultaRepositorioConLosParametros() {
        // Arrange
        Repository
            .Setup(r => r.GetAllAsync("Logitech", 2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Success<(IReadOnlyList<Proveedor> Proveedores, int Total), DomainErrors>(
                    (new List<Proveedor>(), 0)));

        // Act
        var result = await Service.GetAllAsync("Logitech", 2, 5);

        // Assert
        result.IsSuccess.Should().BeTrue();

        Repository.Verify(r => r.GetAllAsync("Logitech", 2, 5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ProveedorExistente_RetornaProveedor() {
        // Arrange
        var proveedor = CreateProveedor();

        Repository
            .Setup(r => r.GetByIdAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Proveedor?, DomainErrors>(proveedor));

        // Act
        var result = await Service.GetByIdAsync(proveedor.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(proveedor.Id);
        result.Value.Nombre.Should().Be(proveedor.Nombre);
    }

    [Test]
    public async Task CreateAsync_DatosValidos_CreaYRetornaProveedor() {
        // Arrange
        var request = CreateRequest();
        var proveedor = CreateProveedor();

        Repository
            .Setup(r => r.ExistsByNameAsync("Logitech", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(false));

        Repository
            .Setup(r => r.AddAsync(It.Is<Proveedor>(p => p.Nombre == "Logitech"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Proveedor, DomainErrors>(proveedor));

        // Act
        var result = await Service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(proveedor.Id);
        result.Value.Nombre.Should().Be(proveedor.Nombre);
        result.Value.Pais.Should().Be("Suiza");

        Repository.Verify(
            r => r.AddAsync(It.Is<Proveedor>(p => p.Nombre == "Logitech"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task UpdateAsync_ProveedorExistente_ActualizaYRetornaProveedor() {
        // Arrange
        var proveedor = CreateProveedor();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Proveedor?, DomainErrors>(proveedor));

        Repository
            .Setup(r => r.ExistsByNameAsync("Funko", proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(false));

        Repository
            .Setup(r => r.UpdateAsync(It.IsAny<Proveedor>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Proveedor p, CancellationToken _) => Result.Success<Proveedor, DomainErrors>(p));

        // Act
        var result = await Service.UpdateAsync(proveedor.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(proveedor.Id);
        result.Value.Nombre.Should().Be("Funko");
        result.Value.Pais.Should().Be("Estados Unidos");

        Repository.Verify(
            r => r.UpdateAsync(It.Is<Proveedor>(p => p.Id == proveedor.Id && p.Nombre == "Funko"),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ProveedorExistente_EliminaYRetornaTrue() {
        // Arrange
        var proveedor = CreateProveedor();

        Repository
            .Setup(r => r.GetByIdAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Proveedor?, DomainErrors>(proveedor));

        Repository
            .Setup(r => r.DeleteAsync(proveedor, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var result = await Service.DeleteAsync(proveedor.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        Repository.Verify(r => r.DeleteAsync(proveedor, It.IsAny<CancellationToken>()), Times.Once);
    }
}
