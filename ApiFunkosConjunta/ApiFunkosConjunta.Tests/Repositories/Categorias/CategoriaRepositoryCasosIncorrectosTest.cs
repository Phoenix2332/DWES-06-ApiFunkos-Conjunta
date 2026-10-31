using ApiFunkosConjunta.Errors;
using FluentAssertions;

namespace ApiFunkosConjunta.Tests.Repositories.Categorias;

[TestFixture]
public class CategoriaRepositoryCasosIncorrectosTest : CategoriaRepositoryTestBase {
    [Test]
    public async Task GetByIdAsync_CategoriaInexistente_RetornaNull() {
        // Arrange
        const string Id = "abcdefghijk";

        // Act
        var result = await Repository.GetByIdAsync(Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Test]
    public async Task GetAllAsync_SinCategoriasCoincidentes_RetornaListaVacia() {
        // Arrange
        await Context.Categorias.AddAsync(CreateCategoria());
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.GetAllAsync("Películas", 1, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Categorias.Should().BeEmpty();
        result.Value.Total.Should().Be(0);
    }

    [Test]
    public async Task ExistsByNameAsync_CategoriaInexistente_RetornaFalse() {
        // Act
        var result = await Repository.ExistsByNameAsync("Películas");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Test]
    public async Task ExistsByNameAsync_ExcluyendoCategoriaExistente_RetornaFalse() {
        // Arrange
        var categoria = CreateCategoria();

        await Context.Categorias.AddAsync(categoria);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.ExistsByNameAsync("Videojuegos", categoria.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
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
        var result = await Repository.GetByIdAsync("abcdefghijk");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetByIdError>();
    }

    [Test]
    public async Task AddAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var categoria = CreateCategoria();

        await Context.DisposeAsync();

        // Act
        var result = await Repository.AddAsync(categoria);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.CreationError>();
    }

    [Test]
    public async Task UpdateAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var categoria = CreateCategoria();

        await Context.DisposeAsync();

        // Act
        var result = await Repository.UpdateAsync(categoria);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.UpdatedError>();
    }

    [Test]
    public async Task DeleteAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var categoria = CreateCategoria();

        await Context.DisposeAsync();

        // Act
        var result = await Repository.DeleteAsync(categoria);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.DeletedError>();
    }

    [Test]
    public async Task ExistsByNameAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        await Context.DisposeAsync();

        // Act
        var result = await Repository.ExistsByNameAsync("Videojuegos");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.ExistsByNameError>();
    }
}