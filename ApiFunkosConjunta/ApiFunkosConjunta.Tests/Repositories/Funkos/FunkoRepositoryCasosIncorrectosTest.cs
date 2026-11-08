using ApiFunkosConjunta.Errors;
using FluentAssertions;

namespace ApiFunkosConjunta.Tests.Repositories.Funkos;

[TestFixture]
public class FunkoRepositoryCasosIncorrectosTest : FunkoRepositoryTestBase {
    [Test]
    public async Task GetByIdAsync_FunkoInexistente_RetornaNull() {
        // Arrange
        const int Id = 999;

        // Act
        var result = await Repository.GetByIdAsync(Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Test]
    public async Task GetAllAsync_SinFunkosCoincidentes_RetornaListaVacia() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();

        await Context.Funkos.AddAsync(CreateFunko(categoriaId, usuarioId));
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.GetAllAsync("Spider-Man", 1, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Funkos.Should().BeEmpty();
        result.Value.Total.Should().Be(0);
    }

    [Test]
    public async Task GetAllAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        await Context.DisposeAsync();

        // Act
        var result = await Repository.GetAllAsync(null, 1, 10);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetAllError>();
    }

    [Test]
    public async Task GetByIdAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        await Context.DisposeAsync();

        // Act
        var result = await Repository.GetByIdAsync(1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetByIdError>();
    }

    [Test]
    public async Task AddAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();
        var funko = CreateFunko(categoriaId, usuarioId);

        await Context.DisposeAsync();

        await Context.DisposeAsync();

        // Act
        var result = await Repository.AddAsync(funko);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.CreationError>();
    }

    [Test]
    public async Task UpdateAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();
        var funko = CreateFunko(categoriaId, usuarioId);

        await Context.DisposeAsync();

        await Context.DisposeAsync();

        // Act
        var result = await Repository.UpdateAsync(funko);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.UpdatedError>();
    }

    [Test]
    public async Task DeleteAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();
        var funko = CreateFunko(categoriaId, usuarioId);

        await Context.DisposeAsync();

        await Context.DisposeAsync();

        // Act
        var result = await Repository.DeleteAsync(funko);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.DeletedError>();
    }

    [Test]
    public async Task ExistsByCategoriaIdAsync_SinFunkosAsociados_RetornaFalse() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();

        // Act
        var result = await Repository.ExistsByCategoriaIdAsync(categoriaId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Test]
    public async Task ExistsByProveedorIdAsync_SinFunkosAsociados_RetornaFalse() {
        // Arrange
        const string ProveedorId = "64b000000000000000000001";

        // Act
        var result = await Repository.ExistsByProveedorIdAsync(ProveedorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Test]
    public async Task ExistsByCategoriaIdAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        await Context.DisposeAsync();

        // Act
        var result = await Repository.ExistsByCategoriaIdAsync("abcdefghijk");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task ExistsByProveedorIdAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        await Context.DisposeAsync();

        // Act
        var result = await Repository.ExistsByProveedorIdAsync("64b000000000000000000001");

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}