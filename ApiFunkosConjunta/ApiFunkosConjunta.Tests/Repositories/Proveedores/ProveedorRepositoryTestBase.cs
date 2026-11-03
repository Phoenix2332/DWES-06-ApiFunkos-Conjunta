using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Repositories.Implementations;
using MongoDB.Bson;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace ApiFunkosConjunta.Tests.Repositories.Proveedores;

public abstract class ProveedorRepositoryTestBase {
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder("mongo:8")
        .Build();

#pragma warning disable NUnit1032
    protected IMongoClient Client = null!;
#pragma warning restore NUnit1032
    protected IMongoDatabase Database = null!;
    protected IMongoCollection<Proveedor> Collection = null!;
    protected ProveedorRepository Repository = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp() {
        await _mongoContainer.StartAsync();
    }

    [SetUp]
    public async Task SetUp() {
        Client = new MongoClient(_mongoContainer.GetConnectionString());
        Database = Client.GetDatabase("ApiFunkosTests");
        Collection = Database.GetCollection<Proveedor>("proveedores");

        // Cada test arranca con la coleccion vacia: si no, los datos de un test contaminan el siguiente.
        await Collection.DeleteManyAsync(FilterDefinition<Proveedor>.Empty);

        Repository = new ProveedorRepository(Collection);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown() {
        await _mongoContainer.DisposeAsync();
    }

    /// <summary>
    /// Repositorio apuntando a un servidor inalcanzable: las operaciones fallan y el
    /// repositorio devuelve su error de dominio en lugar de propagar la excepcion.
    /// </summary>
    protected static ProveedorRepository CreateFailingRepository() {
        var settings = new MongoClientSettings {
            Server = new MongoServerAddress("127.0.0.1", 1),
            ServerSelectionTimeout = TimeSpan.FromMilliseconds(500)
        };

        var client = new MongoClient(settings);

        return new ProveedorRepository(
            client.GetDatabase("ApiFunkosTests").GetCollection<Proveedor>("proveedores"));
    }

    protected static Proveedor CreateProveedor(string nombre = "Logitech") {
        return new Proveedor {
            Nombre = nombre,
            Contacto = "exportaciones@logitech.com",
            Pais = "Suiza"
        };
    }
}
