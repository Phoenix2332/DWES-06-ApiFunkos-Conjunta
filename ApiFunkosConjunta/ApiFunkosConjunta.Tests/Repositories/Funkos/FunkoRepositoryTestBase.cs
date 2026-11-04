using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace ApiFunkosConjunta.Tests.Repositories.Funkos;

public abstract class FunkoRepositoryTestBase {
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:18")
            .WithDatabase("ApiFunkosTests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    protected AppDbContext Context = null!;
    protected FunkoRepository Repository = null!;

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

        Repository = new FunkoRepository(Context);
    }

    [TearDown]
    public async Task TearDown() {
        await Context.DisposeAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown() {
        await _postgresContainer.DisposeAsync();
    }

    protected async Task<string> CreateCategoriaAsync() {
        var categoria = new Categoria {
            Nombre = "Videojuegos"
        };

        await Context.Categorias.AddAsync(categoria);
        await Context.SaveChangesAsync();

        return categoria.Id;
    }

    protected static Funko CreateFunko(string categoriaId, string nombre = "Funko de prueba") {
        return new Funko {
            Nombre = nombre,
            CategoriaId = categoriaId,
            ProveedorId = "64b000000000000000000001",
            Stock = 10,
            Precio = 19.99m,
            Imagen = "imagenes/funko-prueba.png"
        };
    }
}