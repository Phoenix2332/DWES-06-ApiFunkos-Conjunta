using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace ApiFunkosConjunta.Tests.Repositories.Categorias;

public abstract class CategoriaRepositoryTestBase {
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:18")
            .WithDatabase("ApiFunkosTests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    protected AppDbContext Context = null!;
    protected CategoriaRepository Repository = null!;

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

        Repository = new CategoriaRepository(Context);
    }

    [TearDown]
    public async Task TearDown() {
        await Context.DisposeAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown() {
        await _postgresContainer.DisposeAsync();
    }

    protected static Categoria CreateCategoria(string nombre = "Videojuegos") {
        return new Categoria {
            Nombre = nombre
        };
    }
}