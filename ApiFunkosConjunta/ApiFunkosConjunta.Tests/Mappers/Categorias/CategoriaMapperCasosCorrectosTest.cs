using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Mappers;
using ApiFunkosConjunta.Models;
using FluentAssertions;

namespace ApiFunkosConjunta.Tests.Mappers.Categorias;

[TestFixture]
public class CategoriaMapperCasosCorrectosTest {
    [Test]
    public void ToModel_DatosValidos_RetornaCategoria() {
        // Arrange
        var request = new CrearCategoriaDto {
            Nombre = "  Videojuegos  "
        };

        // Act
        var result = CategoriaMapper.ToModel(request);

        // Assert
        result.Should().NotBeNull();
        result.Nombre.Should().Be("Videojuegos");
    }

    [Test]
    public void ToDto_CategoriaValida_RetornaCategoriaDto() {
        // Arrange
        var categoria = new Categoria {
            Id = "abcdefghijk",
            Nombre = "Videojuegos"
        };

        // Act
        var result = CategoriaMapper.ToDto(categoria);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(categoria.Id);
        result.Nombre.Should().Be(categoria.Nombre);
    }

    [Test]
    public void UpdateModel_DatosValidos_ActualizaCategoria() {
        // Arrange
        var categoria = new Categoria {
            Id = "abcdefghijk",
            Nombre = "Videojuegos"
        };

        var request = new ActualizarCategoriaDto {
            Nombre = "  Figuras  "
        };

        // Act
        CategoriaMapper.UpdateModel(categoria, request);

        // Assert
        categoria.Nombre.Should().Be("Figuras");
        categoria.Id.Should().Be("abcdefghijk");
    }
}