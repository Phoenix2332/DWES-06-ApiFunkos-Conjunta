using ApiFunkosConjunta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiFunkosConjunta.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario> {
    public void Configure(EntityTypeBuilder<Usuario> builder) {
        builder.ToTable("usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(u => u.NombreUsuario)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasMaxLength(30);
    }
}