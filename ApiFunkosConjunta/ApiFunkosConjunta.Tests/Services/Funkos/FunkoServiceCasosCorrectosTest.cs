using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Services.Email;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Funkos;

[TestFixture]
public class FunkoServiceCasosCorrectosTest : FunkoServiceTestBase {
    [Test]
    public async Task GetAllAsync_ConFunkos_RetornaFunkosMapeados() {
        // Arrange
        var funkos = new List<Funko> {
            CreateFunko(),
            new() {
                Id = 2,
                Nombre = "Funko de prueba 2",
                CategoriaId = "abcdefghijk",
                ProveedorId = "64b000000000000000000001",
                Stock = 5,
                Precio = 29.99m,
                Imagen = "imagenes/funko-prueba-2.png"
            }
        };

        Repository
            .Setup(r => r.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<(IReadOnlyList<Funko> Funkos, int Total), DomainErrors>((funkos, 2)));

        // Act
        var result = await Service.GetAllAsync(null, 1, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Funkos.Should().HaveCount(2);
        result.Value.Total.Should().Be(2);
        result.Value.Funkos[0].Id.Should().Be(1);
        result.Value.Funkos[0].Nombre.Should().Be("Funko de prueba");
        result.Value.Funkos[1].Nombre.Should().Be("Funko de prueba 2");
    }

    [Test]
    public async Task GetAllAsync_ConFiltroYPaginacion_ConsultaRepositorioConLosParametros() {
        // Arrange
        Repository
            .Setup(r => r.GetAllAsync("Batman", 2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Success<(IReadOnlyList<Funko> Funkos, int Total), DomainErrors>((new List<Funko>(), 0)));

        // Act
        var result = await Service.GetAllAsync("Batman", 2, 5);

        // Assert
        result.IsSuccess.Should().BeTrue();

        Repository.Verify(
            r => r.GetAllAsync("Batman", 2, 5, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_FunkoExistente_RetornaFunko() {
        // Arrange
        var funko = CreateFunko();

        Repository
            .Setup(r => r.GetByIdAsync(funko.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(funko));

        // Act
        var result = await Service.GetByIdAsync(funko.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(funko.Id);
        result.Value.Nombre.Should().Be(funko.Nombre);
        result.Value.CategoriaId.Should().Be(funko.CategoriaId);
        result.Value.ProveedorId.Should().Be(funko.ProveedorId);
        result.Value.Stock.Should().Be(funko.Stock);
        result.Value.Precio.Should().Be(funko.Precio);
        result.Value.Imagen.Should().Be(funko.Imagen);
    }

    [Test]
    public async Task CreateAsync_DatosValidos_CreaYRetornaFunko() {
        // Arrange

        var request = CreateRequest();
        var funko = CreateFunko();

        Repository
            .Setup(r => r.AddAsync(
                It.Is<Funko>(f =>
                    f.Nombre == "Funko de prueba" &&
                    f.CategoriaId == "abcdefghijk" &&
                    f.ProveedorId == "64b000000000000000000001" &&
                    f.UsuarioId == UsuarioId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko, DomainErrors>(funko));

        // Act
        var result = await Service.CreateAsync(request, UsuarioId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(funko.Id);
        result.Value.Nombre.Should().Be(funko.Nombre);
        result.Value.Precio.Should().Be(funko.Precio);

        Repository.Verify(r => r.AddAsync(
                It.Is<Funko>(f => f.Nombre == "Funko de prueba"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task CreateAsync_DatosValidos_EncolaElAvisoAlAdmin() {
        // Arrange

        var request = CreateRequest();
        var funko = CreateFunko();

        Repository
            .Setup(r => r.AddAsync(It.IsAny<Funko>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko, DomainErrors>(funko));

        // Act
        var result = await Service.CreateAsync(request, UsuarioId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        EmailService.Verify(
            e => e.EnqueueEmailAsync(It.Is<EmailMessage>(m =>
                m.To == AdminEmail &&
                m.Subject == "Nuevo funko: Funko de prueba" &&
                m.IsHtml &&
                m.Body.Contains("Funko de prueba"))),
            Times.Once);
    }

    [Test]
    public async Task UpdateAsync_FunkoExistente_ActualizaYRetornaFunko() {
        // Arrange
        var funko = CreateFunko();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(funko.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(funko));

        Repository
            .Setup(r => r.UpdateAsync(It.IsAny<Funko>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Funko f, CancellationToken _) => Result.Success<Funko, DomainErrors>(f));

        // Act
        var result = await Service.UpdateAsync(funko.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(funko.Id);
        result.Value.Nombre.Should().Be("Funko actualizado");
        result.Value.Stock.Should().Be(25);
        result.Value.Precio.Should().Be(24.99m);
        result.Value.Imagen.Should().Be("imagenes/funko-actualizado.png");

        Repository.Verify(r => r.UpdateAsync(
                It.Is<Funko>(f =>
                    f.Id == funko.Id &&
                    f.Nombre == "Funko actualizado" &&
                    f.Stock == 25 &&
                    f.Precio == 24.99m),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task DeleteAsync_FunkoExistente_EliminaYRetornaTrue() {
        // Arrange
        var funko = CreateFunko();

        Repository
            .Setup(r => r.GetByIdAsync(funko.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(funko));

        Repository
            .Setup(r => r.DeleteAsync(funko, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var result = await Service.DeleteAsync(funko.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        Repository.Verify(r => r.DeleteAsync(
                funko,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task ExistsByCategoriaIdAsync_ConFunkosAsociados_RetornaTrue() {
        // Arrange
        const string CategoriaId = "abcdefghijk";

        Repository
            .Setup(r => r.ExistsByCategoriaIdAsync(CategoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var result = await Service.ExistsByCategoriaIdAsync(CategoriaId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        Repository.Verify(r => r.ExistsByCategoriaIdAsync(CategoriaId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ExistsByProveedorIdAsync_ConFunkosAsociados_RetornaTrue() {
        // Arrange
        const string ProveedorId = "64b000000000000000000001";

        Repository
            .Setup(r => r.ExistsByProveedorIdAsync(ProveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var result = await Service.ExistsByProveedorIdAsync(ProveedorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        Repository.Verify(r => r.ExistsByProveedorIdAsync(ProveedorId, It.IsAny<CancellationToken>()), Times.Once);
    }
}