using ApiFunkosConjunta.IdGenerators;
using ApiFunkosConjunta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiFunkosConjunta.Data.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria> {
    public void Configure(EntityTypeBuilder<Categoria> builder) {
        builder.ToTable("categorias");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasMaxLength(11)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<YouTubeIdValueGenerator>();

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);
    }
}