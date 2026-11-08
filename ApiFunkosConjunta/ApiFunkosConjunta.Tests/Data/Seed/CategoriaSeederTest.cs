using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Data.Seed;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApiFunkosConjunta.Tests.Data.Seed;

[TestFixture]
public class CategoriaSeederTest : PostgresSeederTestBase {
    [Test]
    public async Task SeedAsync_BaseDeDatosVacia_CreaLasOchoCategorias() {
        // Act
        await CategoriaSeeder.SeedAsync(ServiceProvider);

        // Assert
        using var scope = ServiceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var categorias = await context.Categorias.ToListAsync();

        categorias.Should().HaveCount(8);

        categorias.Select(c => c.Nombre).Should().BeEquivalentTo(
            "Videojuegos",
            "Películas",
            "Series",
            "Anime",
            "Cómics",
            "Música",
            "Deportes",
            "Tecnología"
        );
    }

    [Test]
    public async Task SeedAsync_CategoriasYaExistentes_NoDuplicaRegistros() {
        // Arrange
        await CategoriaSeeder.SeedAsync(ServiceProvider);

        // Act
        await CategoriaSeeder.SeedAsync(ServiceProvider);

        // Assert
        using var scope = ServiceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        (await context.Categorias.CountAsync()).Should().Be(8);
    }
}