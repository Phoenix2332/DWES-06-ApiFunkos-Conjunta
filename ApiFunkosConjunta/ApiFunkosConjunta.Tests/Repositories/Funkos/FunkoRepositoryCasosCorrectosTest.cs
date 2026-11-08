using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ApiFunkosConjunta.Tests.Repositories.Funkos;

[TestFixture]
public class FunkoRepositoryCasosCorrectosTest : FunkoRepositoryTestBase {
    [Test]
    public async Task GetAllAsync_ConFunkos_RetornaFunkosPaginados() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();

        var funkos = new[] {
            CreateFunko(categoriaId, usuarioId, "Batman"),
            CreateFunko(categoriaId, usuarioId, "Darth Vader"),
            CreateFunko(categoriaId, usuarioId, "Spider-Man")
        };

        await Context.Funkos.AddRangeAsync(funkos);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.GetAllAsync(null, 1, 2);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Funkos.Should().HaveCount(2);
        result.Value.Total.Should().Be(3);

        result.Value.Funkos
            .Select(f => f.Nombre)
            .Should()
            .ContainInOrder("Batman", "Darth Vader");
    }

    [Test]
    public async Task GetAllAsync_ConFiltro_RetornaFunkosCoincidentes() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();

        var funkos = new[] {
            CreateFunko(categoriaId, usuarioId, "Batman"),
            CreateFunko(categoriaId, usuarioId, "Batman Beyond"),
            CreateFunko(categoriaId, usuarioId, "Spider-Man")
        };

        await Context.Funkos.AddRangeAsync(funkos);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.GetAllAsync("Batman", 1, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Total.Should().Be(2);

        result.Value.Funkos
            .Select(f => f.Nombre)
            .Should()
            .Contain(["Batman", "Batman Beyond"]);
    }

    [Test]
    public async Task GetByIdAsync_FunkoExistente_RetornaFunko() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();
        var funko = CreateFunko(categoriaId, usuarioId);

        await Context.Funkos.AddAsync(funko);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.GetByIdAsync(funko.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(funko.Id);
        result.Value.Nombre.Should().Be("Funko de prueba");
    }

    [Test]
    public async Task AddAsync_FunkoValido_GuardaFunkoConIdGenerado() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();
        var funko = CreateFunko(categoriaId, usuarioId);

        // Act
        var result = await Repository.AddAsync(funko);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().BeGreaterThan(0);

        var funkoGuardado = await Context.Funkos
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == result.Value.Id);

        funkoGuardado.Should().NotBeNull();
        funkoGuardado.Nombre.Should().Be("Funko de prueba");
        funkoGuardado.Stock.Should().Be(10);
        funkoGuardado.Precio.Should().Be(19.99m);
    }

    [Test]
    public async Task UpdateAsync_FunkoExistente_ActualizaFunko() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();
        var funko = CreateFunko(categoriaId, usuarioId);

        await Context.Funkos.AddAsync(funko);
        await Context.SaveChangesAsync();

        funko.Nombre = "Funko actualizado";
        funko.Stock = 25;
        funko.Precio = 24.99m;

        // Act
        var result = await Repository.UpdateAsync(funko);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var funkoActualizado = await Context.Funkos
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == funko.Id);

        funkoActualizado.Should().NotBeNull();
        funkoActualizado.Nombre.Should().Be("Funko actualizado");
        funkoActualizado.Stock.Should().Be(25);
        funkoActualizado.Precio.Should().Be(24.99m);
    }

    [Test]
    public async Task DeleteAsync_FunkoExistente_EliminaFunko() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();
        var funko = CreateFunko(categoriaId, usuarioId);

        await Context.Funkos.AddAsync(funko);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.DeleteAsync(funko);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        var funkoEliminado = await Context.Funkos
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == funko.Id);

        funkoEliminado.Should().BeNull();
    }

    [Test]
    public async Task ExistsByCategoriaIdAsync_ConFunkosAsociados_RetornaTrue() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();
        var funko = CreateFunko(categoriaId, usuarioId);

        await Context.Funkos.AddAsync(funko);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.ExistsByCategoriaIdAsync(categoriaId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Test]
    public async Task ExistsByProveedorIdAsync_ConFunkosAsociados_RetornaTrue() {
        // Arrange
        var categoriaId = await CreateCategoriaAsync();
        var usuarioId = await CreateUsuarioAsync();
        const string ProveedorId = "64b000000000000000000001";

        var funko = CreateFunko(categoriaId, usuarioId);
        funko.ProveedorId = ProveedorId;

        await Context.Funkos.AddAsync(funko);
        await Context.SaveChangesAsync();

        // Act
        var result = await Repository.ExistsByProveedorIdAsync(ProveedorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }
}