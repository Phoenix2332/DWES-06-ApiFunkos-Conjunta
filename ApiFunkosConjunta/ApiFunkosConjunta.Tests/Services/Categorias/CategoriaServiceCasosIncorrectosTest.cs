using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Categorias;

[TestFixture]
public class CategoriaServiceCasosIncorrectosTest : CategoriaServiceTestBase {
    [Test]
    public async Task GetByIdAsync_CategoriaInexistente_RetornaNull() {
        // Arrange
        const string Id = "abcdefghijk";

        Repository
            .Setup(r => r.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Categoria?, DomainErrors>(null));

        // Act
        var result = await Service.GetByIdAsync(Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ServiceErrors.NotFoundError>();
    }

    [Test]
    public async Task UpdateAsync_CategoriaInexistente_NoActualizaCategoria() {
        // Arrange
        const string Id = "abcdefghijk";
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Categoria?, DomainErrors>(null));

        // Act
        var result = await Service.UpdateAsync(Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ServiceErrors.NotFoundError>();

        Repository.Verify(r => r.UpdateAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_CategoriaInexistente_NoEliminaCategoria() {
        // Arrange
        const string Id = "abcdefghijk";

        Repository
            .Setup(r => r.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Categoria?, DomainErrors>(null));

        // Act
        var result = await Service.DeleteAsync(Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ServiceErrors.NotFoundError>();

        Repository.Verify(r => r.DeleteAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_NombreDuplicado_NoCreaCategoria() {
        // Arrange
        var request = CreateRequest();

        Repository
            .Setup(r => r.ExistsByNameAsync("Videojuegos", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var result = await Service.CreateAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ServiceErrors.DuplicateNameError>();

        Repository.Verify(r => r.AddAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_NombreDuplicado_NoActualizaCategoria() {
        // Arrange
        var categoria = CreateCategoria();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Categoria?, DomainErrors>(categoria));

        Repository
            .Setup(r => r.ExistsByNameAsync("Figuras", categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var result = await Service.UpdateAsync(categoria.Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ServiceErrors.DuplicateNameError>();

        Repository.Verify(r => r.UpdateAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_ErrorAlComprobarNombre_PropagaErrorRepositorio() {
        // Arrange
        var request = CreateRequest();

        Repository
            .Setup(r => r.ExistsByNameAsync("Videojuegos", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(RepositoryError.ExistsByNameError()));

        // Act
        var result = await Service.CreateAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.ExistsByNameError>();

        Repository.Verify(r => r.AddAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GetByIdAsync_ErrorAlConsultarRepositorio_PropagaError() {
        // Arrange
        const string Id = "abcdefghijk";

        Repository
            .Setup(r => r.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Categoria?, DomainErrors>(RepositoryError.GetByIdError()));

        // Act
        var result = await Service.GetByIdAsync(Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetByIdError>();
    }

    [Test]
    public async Task DeleteAsync_ErrorAlEliminar_PropagaErrorRepositorio() {
        // Arrange
        var categoria = CreateCategoria();

        Repository
            .Setup(r => r.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Categoria?, DomainErrors>(categoria));

        Repository
            .Setup(r => r.DeleteAsync(categoria, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(RepositoryError.DeletedError()));

        // Act
        var result = await Service.DeleteAsync(categoria.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.DeletedError>();
    }

    [Test]
    public async Task GetAllAsync_ErrorAlConsultarRepositorio_PropagaError() {
        // Arrange
        Repository
            .Setup(r => r.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Failure<(IReadOnlyList<Categoria> Categorias, int Total), DomainErrors>(
                    RepositoryError.GetAllError()));

        // Act
        var result = await Service.GetAllAsync(null, 1, 10);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetAllError>();
    }

    [Test]
    public async Task CreateAsync_ErrorAlCrear_PropagaErrorRepositorio() {
        // Arrange
        var request = CreateRequest();

        Repository
            .Setup(r => r.ExistsByNameAsync("Videojuegos", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(false));

        Repository
            .Setup(r => r.AddAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Categoria, DomainErrors>(RepositoryError.CreationError()));

        // Act
        var result = await Service.CreateAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.CreationError>();
    }

    [Test]
    public async Task UpdateAsync_ErrorAlConsultarRepositorio_PropagaError() {
        // Arrange
        const string Id = "abcdefghijk";
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Categoria?, DomainErrors>(RepositoryError.GetByIdError()));

        // Act
        var result = await Service.UpdateAsync(Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetByIdError>();

        Repository.Verify(
            r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task UpdateAsync_ErrorAlComprobarNombre_PropagaErrorRepositorio() {
        // Arrange
        var categoria = CreateCategoria();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Categoria?, DomainErrors>(categoria));

        Repository
            .Setup(r => r.ExistsByNameAsync("Figuras", categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(RepositoryError.ExistsByNameError()));

        // Act
        var result = await Service.UpdateAsync(categoria.Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.ExistsByNameError>();

        Repository.Verify(r => r.UpdateAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_ErrorAlActualizar_PropagaErrorRepositorio() {
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
            .ReturnsAsync(Result.Failure<Categoria, DomainErrors>(RepositoryError.UpdatedError()));

        // Act
        var result = await Service.UpdateAsync(categoria.Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.UpdatedError>();
    }
}