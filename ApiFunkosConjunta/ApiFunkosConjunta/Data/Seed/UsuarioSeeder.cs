using ApiFunkosConjunta.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiFunkosConjunta.Data.Seed;

public static class UsuarioSeeder {
    public static async Task SeedAsync(IServiceProvider serviceProvider) {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var username = configuration["Jwt:AdminUsername"];
        var password = configuration["Jwt:AdminPassword"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("No se ha configurado el administrador inicial.");

        var existe = await context.Usuarios
            .AnyAsync(u => u.NombreUsuario == username);

        if (existe)
            return;

        var usuario = new Usuario {
            NombreUsuario = username,
            Rol = "Administrador"
        };

        var passwordHasher = new PasswordHasher<Usuario>();
        usuario.PasswordHash = passwordHasher.HashPassword(usuario, password);

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
    }
}