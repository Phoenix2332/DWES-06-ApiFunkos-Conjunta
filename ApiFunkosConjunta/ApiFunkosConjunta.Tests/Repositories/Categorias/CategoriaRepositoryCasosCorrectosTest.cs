using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ApiFunkosConjunta.Tests.Repositories.Categorias;

[TestFixture]
public class CategoriaRepositoryCasosCorrectosTest : CategoriaRepositoryTestBase {
    [Test]
    public async Task GetAllAsync_ConCategorias_RetornaCategoriasPaginadas() {
        // Arrange
        var categorias = new[] {
            CreateCategoria("Acción"),
            CreateCategoria("Aventura"),
            CreateCategoria("Deportes")
        };

        await Context.Categorias.AddRangeAsync(categorias);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.GetAllAsync(null, 1, 2);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Categorias.Should().HaveCount(2);
        result.Value.Total.Should().Be(3);

        result.Value.Categorias
            .Select(c => c.Nombre)
            .Should()
            .ContainInOrder("Acción", "Aventura");
    }

    [Test]
    public async Task GetAllAsync_ConFiltro_RetornaCategoriasCoincidentes() {
        // Arrange
        var categorias = new[] {
            CreateCategoria(),
            CreateCategoria("Películas"),
            CreateCategoria("Videojuegos retro")
        };

        await Context.Categorias.AddRangeAsync(categorias);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.GetAllAsync("Videojuegos", 1, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Total.Should().Be(2);

        result.Value.Categorias
            .Select(c => c.Nombre)
            .Should()
            .Contain(["Videojuegos", "Videojuegos retro"]);
    }

    [Test]
    public async Task GetByIdAsync_CategoriaExistente_RetornaCategoria() {
        // Arrange
        var categoria = CreateCategoria();

        await Context.Categorias.AddAsync(categoria);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.GetByIdAsync(categoria.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(categoria.Id);
        result.Value.Nombre.Should().Be("Videojuegos");
    }

    [Test]
    public async Task AddAsync_CategoriaValida_GuardaCategoriaConIdGenerado() {
        // Arrange
        var categoria = CreateCategoria();

        // Act
        var result = await Repository.AddAsync(categoria);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeNullOrWhiteSpace();
        result.Value.Id.Should().HaveLength(11);

        var categoriaGuardada = await Context.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == result.Value.Id);

        categoriaGuardada.Should().NotBeNull();
        categoriaGuardada.Nombre.Should().Be("Videojuegos");
    }

    [Test]
    public async Task UpdateAsync_CategoriaExistente_ActualizaCategoria() {
        // Arrange
        var categoria = CreateCategoria();

        await Context.Categorias.AddAsync(categoria);
        await Context.SaveChangesAsync();

        categoria.Nombre = "Figuras";

        // Act
        var result = await Repository.UpdateAsync(categoria);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var categoriaActualizada = await Context.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoria.Id);

        categoriaActualizada.Should().NotBeNull();
        categoriaActualizada.Nombre.Should().Be("Figuras");
    }

    [Test]
    public async Task DeleteAsync_CategoriaExistente_EliminaCategoria() {
        // Arrange
        var categoria = CreateCategoria();

        await Context.Categorias.AddAsync(categoria);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.DeleteAsync(categoria);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        var categoriaEliminada = await Context.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoria.Id);

        categoriaEliminada.Should().BeNull();
    }

    [Test]
    public async Task ExistsByNameAsync_CategoriaExistente_RetornaTrue() {
        // Arrange
        var categoria = CreateCategoria();

        await Context.Categorias.AddAsync(categoria);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.ExistsByNameAsync("Videojuegos");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }
}