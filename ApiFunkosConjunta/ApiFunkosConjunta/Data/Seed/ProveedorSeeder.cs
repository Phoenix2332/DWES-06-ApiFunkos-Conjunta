using ApiFunkosConjunta.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApiFunkosConjunta.Data.Seed;

public static class ProveedorSeeder {
    private static readonly ObjectId _cladellasId = ObjectId.Parse("64b000000000000000000001");
    private static readonly ObjectId _extendedId = ObjectId.Parse("64b000000000000000000002");
    private static readonly ObjectId _kilumioId = ObjectId.Parse("64b000000000000000000003");

    public static async Task SeedAsync(IServiceProvider serviceProvider) {
        using var scope = serviceProvider.CreateScope();

        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

        var proveedores = database.GetCollection<Proveedor>("proveedores");

        var datosIniciales = new[] {
            new Proveedor {
                Id = _cladellasId,
                Nombre = "Grupo Cladellas",
                Contacto = "exportaciones@grupocladellas.com",
                Pais = "España"
            },
            new Proveedor {
                Id = _extendedId,
                Nombre = "Extended Play",
                Contacto = "exportaciones@extendedplay.com",
                Pais = "España"
            },
            new Proveedor {
                Id = _kilumioId,
                Nombre = "Kilumio",
                Contacto = "exportaciones@kilumio.com",
                Pais = "España"
            }
        };

        foreach (var proveedor in datosIniciales)
            await proveedores.ReplaceOneAsync(p => p.Id == proveedor.Id, proveedor,
                new ReplaceOptions { IsUpsert = true });
    }
}