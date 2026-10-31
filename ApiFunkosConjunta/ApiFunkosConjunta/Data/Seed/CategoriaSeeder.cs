using ApiFunkosConjunta.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiFunkosConjunta.Data.Seed;

public static class CategoriaSeeder {
    public static async Task SeedAsync(IServiceProvider serviceProvider) {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureCreatedAsync();

        var nombres = new[] {
            "Videojuegos",
            "Películas",
            "Series",
            "Anime",
            "Cómics",
            "Música",
            "Deportes",
            "Tecnología"
        };

        foreach (var nombre in nombres) {
            var existe = await context.Categorias.AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower());

            if (!existe)
                await context.Categorias.AddAsync(new Categoria {
                    Nombre = nombre
                });
        }

        await context.SaveChangesAsync();
    }
}