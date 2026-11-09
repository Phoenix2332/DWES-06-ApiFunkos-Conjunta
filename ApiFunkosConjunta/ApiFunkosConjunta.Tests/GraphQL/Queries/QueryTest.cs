using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.GraphQL.Queries;
using ApiFunkosConjunta.Services.Interfaces;
using CSharpFunctionalExtensions;
using FluentAssertions;
using HotChocolate;
using Moq;

namespace ApiFunkosConjunta.Tests.GraphQL.Queries;

[TestFixture]
public class QueryTest {
    [SetUp]
    public void SetUp() {
        _funkoService = new Mock<IFunkoService>();
        _categoriaService = new Mock<ICategoriaService>();
        _query = new Query();
    }

    private Mock<IFunkoService> _funkoService = null!;
    private Mock<ICategoriaService> _categoriaService = null!;
    private Query _query = null!;

    private static FunkoDto CreateFunko() {
        return new FunkoDto {
            Id = 1,
            Nombre = "Funko One Piece",
            CategoriaId = "abcdefghijk",
            ProveedorId = "507f1f77bcf86cd799439011",
            Stock = 10,
            Precio = 19.99m,
            Imagen = "imagenes/funko.jpg"
        };
    }

    private static CategoriaDto CreateCategoria() {
        return new CategoriaDto {
            Id = "abcdefghijk",
            Nombre = "Videojuegos"
        };
    }

    private static DomainErrors CreateError() {
        return CatalogoServiceError.ProveedorIdInvalidoError("id-invalido");
    }

    [Test]
    public void Estado_Siempre_RetornaMensajeEsperado() {
        // Act
        var result = _query.Estado();

        // Assert
        result.Should().Be("API Funkos GraphQL funcionando");
    }

    [Test]
    public async Task GetFunkosAsync_ConFunkos_RetornaResultadoPaginado() {
        // Arrange
        var funkos = new List<FunkoDto> {
            CreateFunko(),
            new() {
                Id = 2,
                Nombre = "Funko Naruto"
            }
        };

        _funkoService
            .Setup(s => s.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Success<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>(
                    (funkos, 2)));

        // Act
        var result = await _query.GetFunkosAsync(_funkoService.Object);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(2);
        result.Pagina.Should().Be(1);
        result.TamanoPagina.Should().Be(10);
        result.Items[0].Nombre.Should().Be("Funko One Piece");

        _funkoService.Verify(
            s => s.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetFunkosAsync_ErrorDelServicio_LanzaGraphQLException() {
        // Arrange
        _funkoService
            .Setup(s => s.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Failure<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>(
                    CreateError()));

        // Act
        var act = async () => await _query.GetFunkosAsync(_funkoService.Object);

        // Assert
        await act.Should().ThrowAsync<GraphQLException>();
    }

    [Test]
    public async Task GetFunkoAsync_FunkoExistente_RetornaFunko() {
        // Arrange
        var funko = CreateFunko();

        _funkoService
            .Setup(s => s.GetByIdAsync(funko.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<FunkoDto, DomainErrors>(funko));

        // Act
        var result = await _query.GetFunkoAsync(_funkoService.Object, funko.Id);

        // Assert
        result.Should().BeSameAs(funko);
        result.Id.Should().Be(1);
        result.Nombre.Should().Be("Funko One Piece");

        _funkoService.Verify(
            s => s.GetByIdAsync(funko.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetFunkoAsync_ErrorDelServicio_LanzaGraphQLException() {
        // Arrange
        const int Id = 999;

        _funkoService
            .Setup(s => s.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Failure<FunkoDto, DomainErrors>(CreateError()));

        // Act
        var act = async () => await _query.GetFunkoAsync(_funkoService.Object, Id);

        // Assert
        await act.Should().ThrowAsync<GraphQLException>();
    }

    [Test]
    public async Task GetCategoriasAsync_ConCategorias_RetornaResultadoPaginado() {
        // Arrange
        var categorias = new List<CategoriaDto> {
            CreateCategoria(),
            new() {
                Id = "12345678901",
                Nombre = "Películas"
            }
        };

        _categoriaService
            .Setup(s => s.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Success<(IReadOnlyList<CategoriaDto> Categorias, int Total), DomainErrors>(
                    (categorias, 2)));

        // Act
        var result = await _query.GetCategoriasAsync(_categoriaService.Object);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(2);
        result.Pagina.Should().Be(1);
        result.TamanoPagina.Should().Be(10);
        result.Items[0].Nombre.Should().Be("Videojuegos");

        _categoriaService.Verify(
            s => s.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetCategoriasAsync_ErrorDelServicio_LanzaGraphQLException() {
        // Arrange
        _categoriaService
            .Setup(s => s.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Failure<(IReadOnlyList<CategoriaDto> Categorias, int Total), DomainErrors>(
                    CreateError()));

        // Act
        var act = async () => await _query.GetCategoriasAsync(_categoriaService.Object);

        // Assert
        await act.Should().ThrowAsync<GraphQLException>();
    }

    [Test]
    public async Task GetCategoriaAsync_CategoriaExistente_RetornaCategoria() {
        // Arrange
        var categoria = CreateCategoria();

        _categoriaService
            .Setup(s => s.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        // Act
        var result = await _query.GetCategoriaAsync(_categoriaService.Object, categoria.Id);

        // Assert
        result.Should().BeSameAs(categoria);
        result.Id.Should().Be("abcdefghijk");
        result.Nombre.Should().Be("Videojuegos");

        _categoriaService.Verify(
            s => s.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetCategoriaAsync_ErrorDelServicio_LanzaGraphQLException() {
        // Arrange
        const string Id = "inexistente";

        _categoriaService
            .Setup(s => s.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Failure<CategoriaDto, DomainErrors>(CreateError()));

        // Act
        var act = async () => await _query.GetCategoriaAsync(_categoriaService.Object, Id);

        // Assert
        await act.Should().ThrowAsync<GraphQLException>();
    }
}