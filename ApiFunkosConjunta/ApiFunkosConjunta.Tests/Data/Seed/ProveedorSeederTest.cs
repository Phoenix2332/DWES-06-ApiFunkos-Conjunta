using ApiFunkosConjunta.Data.Seed;
using ApiFunkosConjunta.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace ApiFunkosConjunta.Tests.Data.Seed;

[TestFixture]
public class ProveedorSeederTest {
    [OneTimeSetUp]
    public async Task OneTimeSetUp() {
        await _mongoContainer.StartAsync();
    }

    [SetUp]
    public void SetUp() {
        var client = new MongoClient(_mongoContainer.GetConnectionString());

        _database = client.GetDatabase("ApiFunkosTests");

        _database.DropCollection("proveedores");

        var services = new ServiceCollection();

        services.AddSingleton(_database);

        _serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public async Task TearDown() {
        await _serviceProvider.DisposeAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown() {
        await _mongoContainer.DisposeAsync();
    }

    private readonly MongoDbContainer _mongoContainer =
        new MongoDbBuilder()
            .WithImage("mongo:8")
            .Build();

    private ServiceProvider _serviceProvider = null!;
    private IMongoDatabase _database = null!;

    [Test]
    public async Task SeedAsync_ColeccionVacia_CreaTresProveedores() {
        // Act
        await ProveedorSeeder.SeedAsync(_serviceProvider);

        // Assert
        var proveedores = await _database
            .GetCollection<Proveedor>("proveedores")
            .Find(FilterDefinition<Proveedor>.Empty)
            .ToListAsync();

        proveedores.Should().HaveCount(3);

        proveedores.Select(p => p.Nombre).Should().BeEquivalentTo(
            "Grupo Cladellas",
            "Extended Play",
            "Kilumio"
        );

        proveedores.Should().OnlyContain(p => p.Id != ObjectId.Empty);
    }

    [Test]
    public async Task SeedAsync_ProveedoresExistentes_ActualizaSinDuplicar() {
        // Arrange
        var coleccion = _database.GetCollection<Proveedor>("proveedores");

        await coleccion.InsertOneAsync(new Proveedor {
            Id = ObjectId.Parse("64b000000000000000000001"),
            Nombre = "Nombre antiguo",
            Contacto = "antiguo@ejemplo.com",
            Pais = "Francia"
        });

        // Act
        await ProveedorSeeder.SeedAsync(_serviceProvider);

        // Assert
        var proveedores = await coleccion
            .Find(FilterDefinition<Proveedor>.Empty)
            .ToListAsync();

        proveedores.Should().HaveCount(3);

        var cladellas = proveedores.Single(p =>
            p.Id == ObjectId.Parse("64b000000000000000000001"));

        cladellas.Nombre.Should().Be("Grupo Cladellas");
        cladellas.Contacto.Should().Be("exportaciones@grupocladellas.com");
        cladellas.Pais.Should().Be("España");
    }
}