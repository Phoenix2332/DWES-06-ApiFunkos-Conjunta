using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace ApiFunkosConjunta.Tests.Services.Auth;

public abstract class AuthServiceTestBase {
    protected const string JwtKey = "ClaveDePruebaSeguraDeAlMenos32Caracteres!";
    protected const string JwtIssuer = "ApiFunkosTests";
    protected const string JwtAudience = "ApiFunkosTests";

    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:18")
            .WithDatabase("ApiFunkosTests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    protected AppDbContext Context = null!;
    protected AuthService Service = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp() {
        await _postgresContainer.StartAsync();
    }

    [SetUp]
    public async Task SetUp() {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .Options;

        Context = new AppDbContext(options);

        await Context.Database.EnsureDeletedAsync();
        await Context.Database.EnsureCreatedAsync();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["Jwt:Key"] = JwtKey,
                ["Jwt:Issuer"] = JwtIssuer,
                ["Jwt:Audience"] = JwtAudience
            })
            .Build();

        Service = new AuthService(Context, configuration);
    }

    [TearDown]
    public async Task TearDown() {
        await Context.DisposeAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown() {
        await _postgresContainer.DisposeAsync();
    }

    protected async Task<Usuario> CreateUsuarioAsync(string username = "usuario-test",
        string password = "password-test", string rol = "Usuario") {
        var usuario = new Usuario {
            NombreUsuario = username,
            Rol = rol
        };

        usuario.PasswordHash = new PasswordHasher<Usuario>().HashPassword(usuario, password);

        await Context.Usuarios.AddAsync(usuario);
        await Context.SaveChangesAsync();

        return usuario;
    }
}