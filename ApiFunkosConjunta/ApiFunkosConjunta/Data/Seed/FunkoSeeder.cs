using ApiFunkosConjunta.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiFunkosConjunta.Data.Seed;

public static class FunkoSeeder {
    public static async Task SeedAsync(IServiceProvider serviceProvider) {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var administrador = await context.Usuarios
            .FirstOrDefaultAsync(u => u.Rol == "Administrador");

        if (administrador is null)
            throw new InvalidOperationException("No existe un administrador inicial.");

        var categorias = await context.Categorias.ToDictionaryAsync(c => c.Nombre);

        var datosIniciales = new[] {
            new Funko {
                Nombre = "Spider-Man",
                CategoriaId = categorias["Cómics"].Id,
                ProveedorId = "64b000000000000000000001",
                UsuarioId = administrador.Id,
                Stock = 15,
                Precio = 14.99m,
                Imagen = "imagenes/funkos/spider-man.png"
            },
            new Funko {
                Nombre = "Batman",
                CategoriaId = categorias["Cómics"].Id,
                ProveedorId = "64b000000000000000000002",
                UsuarioId = administrador.Id,
                Stock = 10,
                Precio = 16.99m,
                Imagen = "imagenes/funkos/batman.png"
            },
            new Funko {
                Nombre = "Naruto Uzumaki",
                CategoriaId = categorias["Anime"].Id,
                ProveedorId = "64b000000000000000000003",
                UsuarioId = administrador.Id,
                Stock = 20,
                Precio = 15.99m,
                Imagen = "imagenes/funkos/naruto.png"
            },
            new Funko {
                Nombre = "Darth Vader",
                CategoriaId = categorias["Películas"].Id,
                ProveedorId = "64b000000000000000000001",
                UsuarioId = administrador.Id,
                Stock = 8,
                Precio = 18.99m,
                Imagen = "imagenes/funkos/darth-vader.png"
            },
            new Funko {
                Nombre = "Geralt de Rivia",
                CategoriaId = categorias["Videojuegos"].Id,
                ProveedorId = "64b000000000000000000002",
                UsuarioId = administrador.Id,
                Stock = 12,
                Precio = 17.99m,
                Imagen = "imagenes/funkos/geralt.png"
            },
            new Funko {
                Nombre = "Eleven",
                CategoriaId = categorias["Series"].Id,
                ProveedorId = "64b000000000000000000003",
                UsuarioId = administrador.Id,
                Stock = 6,
                Precio = 13.99m,
                Imagen = "imagenes/funkos/eleven.png"
            }
        };

        foreach (var funko in datosIniciales) {
            var existe = await context.Funkos
                .AnyAsync(f => f.Nombre.ToLower() == funko.Nombre.ToLower());

            if (!existe)
                await context.Funkos.AddAsync(funko);
        }

        await context.SaveChangesAsync();
    }
}