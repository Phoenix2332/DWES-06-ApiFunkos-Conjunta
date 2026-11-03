using ApiFunkosConjunta.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApiFunkosConjunta.Data.Seed;

public static class ProveedorSeeder {
    // IDs fijos: si fueran aleatorios, los funkos referenciarían proveedores
    // que ya no existen tras cada reinicio.
    private static readonly ObjectId LogitechId = ObjectId.Parse("64b000000000000000000001");
    private static readonly ObjectId IkeaId = ObjectId.Parse("64b000000000000000000002");
    private static readonly ObjectId FunkoId = ObjectId.Parse("64b000000000000000000003");

    public static async Task SeedAsync(IServiceProvider serviceProvider) {
        using var scope = serviceProvider.CreateScope();

        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

        var proveedores = database.GetCollection<Proveedor>("proveedores");

        await proveedores.DeleteManyAsync(FilterDefinition<Proveedor>.Empty);

        await proveedores.InsertManyAsync(new[] {
            new Proveedor {
                Id = LogitechId,
                Nombre = "Logitech",
                Contacto = "exportaciones@logitech.com",
                Pais = "Suiza"
            },
            new Proveedor {
                Id = IkeaId,
                Nombre = "Ikea",
                Contacto = "proveedores@ikea.com",
                Pais = "Suecia"
            },
            new Proveedor {
                Id = FunkoId,
                Nombre = "Funko",
                Contacto = "ventas@funko.com",
                Pais = "Estados Unidos"
            }
        });
    }
}
