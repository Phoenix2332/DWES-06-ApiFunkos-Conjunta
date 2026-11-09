using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Infrastructure.Persistence;
using ApiFunkosConjunta.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;

namespace ApiFunkosConjunta.Tests.Infrastructure;

[TestFixture]
public class PersistenceExtensionsTest {
    private static IConfiguration CrearConfiguracion() {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Port=5432;Database=ApiFunkos;Username=postgres;Password=postgres",
                ["ConnectionStrings:Redis"] = "localhost:6379",
                ["MongoDB:ConnectionString"] = "mongodb://admin:admin@localhost:27017/?authSource=admin",
                ["MongoDB:DatabaseName"] = "ApiFunkos"
            })
            .Build();
    }

    [Test]
    public void AddPersistence_RegistraElDbContextYLasConexiones() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddPersistence(CrearConfiguracion());

        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<AppDbContext>().Should().NotBeNull();
        provider.GetService<IDistributedCache>().Should().NotBeNull();
        provider.GetService<IMongoClient>().Should().NotBeNull();
        provider.GetService<IMongoDatabase>().Should().NotBeNull();
        provider.GetService<IMongoCollection<Proveedor>>().Should().NotBeNull();
    }

    [Test]
    public void AddPersistence_RegistraElMongoClientComoSingleton() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddPersistence(CrearConfiguracion());

        var descriptor = services.First(s => s.ServiceType == typeof(IMongoClient));

        // Assert
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }
}
