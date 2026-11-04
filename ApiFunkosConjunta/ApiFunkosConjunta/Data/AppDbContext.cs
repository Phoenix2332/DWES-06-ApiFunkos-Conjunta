using ApiFunkosConjunta.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiFunkosConjunta.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Funko> Funkos => Set<Funko>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}