using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Models;

namespace ApiFunkosConjunta.Mappers;

public static class CategoriaMapper {
    public static Categoria ToModel(CrearCategoriaDto dto) {
        return new Categoria {
            Nombre = dto.Nombre.Trim()
        };
    }

    public static CategoriaDto ToDto(Categoria categoria) {
        return new CategoriaDto {
            Id = categoria.Id,
            Nombre = categoria.Nombre
        };
    }

    public static void UpdateModel(Categoria categoria, ActualizarCategoriaDto dto) {
        categoria.Nombre = dto.Nombre.Trim();
    }
}