using ApiFunkosConjunta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiFunkosConjunta.Data.Configurations;

public class FunkoConfiguration : IEntityTypeConfiguration<Funko> {
    public void Configure(EntityTypeBuilder<Funko> builder) {
        builder.ToTable("funkos");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .ValueGeneratedOnAdd();

        builder.Property(f => f.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.CategoriaId)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(f => f.ProveedorId)
            .IsRequired()
            .HasMaxLength(24);

        builder.Property(f => f.Stock)
            .IsRequired();

        builder.Property(f => f.Precio)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(f => f.Imagen)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne<Categoria>()
            .WithMany()
            .HasForeignKey(f => f.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Usuario)
            .WithMany()
            .HasForeignKey(f => f.UsuarioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}