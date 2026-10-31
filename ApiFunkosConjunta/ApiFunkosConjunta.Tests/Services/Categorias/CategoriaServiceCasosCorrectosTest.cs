using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Categorias;

[TestFixture]
public class CategoriaServiceCasosCorrectosTest : CategoriaServiceTestBase {
    [Test]
    public async Task GetAllAsync_ConCategorias_RetornaCategoriasMapeadas() {
        // Arrange
        var categorias = new List<Categoria> {
            CreateCategoria(),
            new() { Id = "12345678901", Nombre = "Películas" }
        };

        Repository
            .Setup(r => r.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Success<(IReadOnlyList<Categoria> Categorias, int Total), DomainErrors>((categorias, 2)));

        // Act
        var result = await Service.GetAllAsync(null, 1, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Categorias.Should().HaveCount(2);
        result.Value.Total.Should().Be(2);
        result.Value.Categorias[0].Id.Should().Be("abcdefghijk");
        result.Value.Categorias[0].Nombre.Should().Be("Videojuegos");
        result.Value.Categorias[1].Nombre.Should().Be("Películas");
    }

    [Test]
    public async Task GetAllAsync_ConFiltroYpaginacion_ConsultaRepositorioConLosParametros() {
        // Arrange
        Repository
            .Setup(r => r.GetAllAsync("Videojuegos", 2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Success<(IReadOnlyList<Categoria> Categorias, int Total), DomainErrors>((new List<Categoria>(),
                    0)));

        // Act
        var result = await Service.GetAllAsync("Videojuegos", 2, 5);

        // Assert
        result.IsSuccess.Should().BeTrue();

        Repository.Verify(r => r.GetAllAsync("Videojuegos", 2, 5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_CategoriaExistente_RetornaCategoria() {
        // Arrange
        var categoria = CreateCategoria();

        Repository
            .Setup(r => r.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Categoria?, DomainErrors>(categoria));

        // Act
        var result = await Service.GetByIdAsync(categoria.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(categoria.Id);
        result.Value.Nombre.Should().Be(categoria.Nombre);
    }

    [Test]
    public async Task CreateAsync_DatosValidos_CreaYRetornaCategoria() {
        // Arrange
        var request = CreateRequest();
        var categoria = CreateCategoria();

        Repository
            .Setup(r => r.ExistsByNameAsync("Videojuegos", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(false));

        Repository
            .Setup(r => r.AddAsync(It.Is<Categoria>(c => c.Nombre == "Videojuegos"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Categoria, DomainErrors>(categoria));

        // Act
        var result = await Service.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(categoria.Id);
        result.Value.Nombre.Should().Be(categoria.Nombre);

        Repository.Verify(
            r => r.AddAsync(It.Is<Categoria>(c => c.Nombre == "Videojuegos"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task UpdateAsync_CategoriaExistente_ActualizaYRetornaCategoria() {
        // Arrange
        var categoria = CreateCategoria();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Categoria?, DomainErrors>(categoria));

        Repository
            .Setup(r => r.ExistsByNameAsync("Figuras", categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(false));

        Repository
            .Setup(r => r.UpdateAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria c, CancellationToken _) => Result.Success<Categoria, DomainErrors>(c));

        // Act
        var result = await Service.UpdateAsync(categoria.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(categoria.Id);
        result.Value.Nombre.Should().Be("Figuras");

        Repository.Verify(
            r => r.UpdateAsync(It.Is<Categoria>(c => c.Id == categoria.Id && c.Nombre == "Figuras"),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_CategoriaExistente_EliminaYRetornaTrue() {
        // Arrange
        var categoria = CreateCategoria();

        Repository
            .Setup(r => r.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Categoria?, DomainErrors>(categoria));

        Repository
            .Setup(r => r.DeleteAsync(categoria, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var result = await Service.DeleteAsync(categoria.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        Repository.Verify(r => r.DeleteAsync(categoria, It.IsAny<CancellationToken>()), Times.Once);
    }
}