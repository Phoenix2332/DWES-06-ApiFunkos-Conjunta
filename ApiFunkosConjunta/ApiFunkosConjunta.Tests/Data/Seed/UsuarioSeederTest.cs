using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Data.Seed;
using ApiFunkosConjunta.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiFunkosConjunta.Tests.Data.Seed;

[TestFixture]
public class UsuarioSeederTest : PostgresSeederTestBase {
    [TearDown]
    public async Task DisposeConfiguredProvider() {
        if (_serviceProviderConfigurado is not null)
            await _serviceProviderConfigurado.DisposeAsync();
    }

    private ServiceProvider _serviceProviderConfigurado = null!;

    private void ConfigurarCredenciales(string? username, string? password) {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["Jwt:AdminUsername"] = username,
                ["Jwt:AdminPassword"] = password
            })
            .Build();

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(ConnectionString));

        services.AddSingleton<IConfiguration>(configuration);

        _serviceProviderConfigurado = services.BuildServiceProvider();
    }

    [TestCase(null, "password")]
    [TestCase("", "password")]
    [TestCase("administrador", null)]
    [TestCase("administrador", "")]
    public async Task SeedAsync_CredencialesNoConfiguradas_LanzaExcepcion(
        string? username,
        string? password) {
        // Arrange
        ConfigurarCredenciales(username, password);

        // Act
        var accion = async () =>
            await UsuarioSeeder.SeedAsync(_serviceProviderConfigurado);

        // Assert
        await accion.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("No se ha configurado el administrador inicial.");
    }

    [Test]
    public async Task SeedAsync_AdministradorNoExistente_CreaAdministrador() {
        // Arrange
        ConfigurarCredenciales("administrador", "password-test");

        // Act
        await UsuarioSeeder.SeedAsync(_serviceProviderConfigurado);

        // Assert
        using var scope = _serviceProviderConfigurado.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var usuario = await context.Usuarios.SingleAsync();

        usuario.NombreUsuario.Should().Be("administrador");
        usuario.Rol.Should().Be("Administrador");
        usuario.PasswordHash.Should().NotBeNullOrWhiteSpace();

        var verificador = new PasswordHasher<Usuario>();

        verificador.VerifyHashedPassword(
            usuario,
            usuario.PasswordHash,
            "password-test"
        ).Should().Be(PasswordVerificationResult.Success);
    }

    [Test]
    public async Task SeedAsync_AdministradorExistente_NoCreaOtro() {
        // Arrange
        ConfigurarCredenciales("administrador", "password-test");

        using (var scope = ServiceProvider.CreateScope()) {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Usuarios.Add(new Usuario {
                NombreUsuario = "administrador",
                Rol = "Administrador",
                PasswordHash = "hash-existente"
            });

            await context.SaveChangesAsync();
        }

        // Act
        await UsuarioSeeder.SeedAsync(_serviceProviderConfigurado);

        // Assert
        using var assertScope = ServiceProvider.CreateScope();

        var assertContext = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();

        (await assertContext.Usuarios.CountAsync()).Should().Be(1);
    }
}