using ApiFunkosConjunta.Dto.Categorias;
using Swashbuckle.AspNetCore.Filters;

namespace ApiFunkosConjunta.Common.Examples;

/// <summary>
/// Ejemplo de peticion para crear una categoria (apartado 24.8 del tema).
/// </summary>
public class CrearCategoriaDtoExample : IExamplesProvider<CrearCategoriaDto> {
    /// <summary>
    /// Devuelve una categoria de ejemplo que Swagger UI muestra como cuerpo de la peticion.
    /// </summary>
    public CrearCategoriaDto GetExamples() {
        return new CrearCategoriaDto {
            Nombre = "Videojuegos"
        };
    }
}
