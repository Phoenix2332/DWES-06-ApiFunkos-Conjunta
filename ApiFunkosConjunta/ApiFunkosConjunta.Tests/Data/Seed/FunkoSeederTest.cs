using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Data.Seed;
using ApiFunkosConjunta.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApiFunkosConjunta.Tests.Data.Seed;

[TestFixture]
public class FunkoSeederTest : PostgresSeederTestBase {
    private async Task PrepararDatosIniciales() {
        using var scope = ServiceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Usuarios.Add(new Usuario {
            NombreUsuario = "administrador",
            Rol = "Administrador",
            PasswordHash = "hash-test"
        });

        context.Categorias.AddRange(
            new Categoria { Nombre = "Videojuegos" },
            new Categoria { Nombre = "Películas" },
            new Categoria { Nombre = "Series" },
            new Categoria { Nombre = "Anime" },
            new Categoria { Nombre = "Cómics" },
            new Categoria { Nombre = "Música" },
            new Categoria { Nombre = "Deportes" },
            new Categoria { Nombre = "Tecnología" }
        );

        await context.SaveChangesAsync();
    }

    [Test]
    public async Task SeedAsync_SinAdministrador_LanzaExcepcion() {
        // Act
        var accion = async () =>
            await FunkoSeeder.SeedAsync(ServiceProvider);

        // Assert
        await accion.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("No existe un administrador inicial.");
    }

    [Test]
    public async Task SeedAsync_ConAdministradorYCategorias_CreaSeisFunkos() {
        // Arrange
        await PrepararDatosIniciales();

        // Act
        await FunkoSeeder.SeedAsync(ServiceProvider);

        // Assert
        using var scope = ServiceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var funkos = await context.Funkos.ToListAsync();

        funkos.Should().HaveCount(6);

        funkos.Select(f => f.Nombre).Should().BeEquivalentTo(
            "Spider-Man",
            "Batman",
            "Naruto Uzumaki",
            "Darth Vader",
            "Geralt de Rivia",
            "Eleven"
        );

        funkos.Should().OnlyContain(f => f.UsuarioId > 0);
        funkos.Should().OnlyContain(f => !string.IsNullOrWhiteSpace(f.Imagen));
    }

    [Test]
    public async Task SeedAsync_FunkosYaExistentes_NoDuplicaRegistros() {
        // Arrange
        await PrepararDatosIniciales();

        await FunkoSeeder.SeedAsync(ServiceProvider);

        // Act
        await FunkoSeeder.SeedAsync(ServiceProvider);

        // Assert
        using var scope = ServiceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        (await context.Funkos.CountAsync()).Should().Be(6);
    }
}