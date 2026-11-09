using ApiFunkosConjunta.Dto.Funkos;
using Swashbuckle.AspNetCore.Filters;

namespace ApiFunkosConjunta.Common.Examples;

/// <summary>
/// Ejemplo de peticion para crear un Funko (apartado 24.8 del tema).
/// </summary>
public class CrearFunkoDtoExample : IExamplesProvider<CrearFunkoDto> {
    /// <summary>
    /// Devuelve un Funko de ejemplo que Swagger UI muestra como cuerpo de la peticion.
    /// </summary>
    public CrearFunkoDto GetExamples() {
        return new CrearFunkoDto {
            Nombre = "Funko Pop! Batman",
            CategoriaId = "D2M_Lk6kKe6",
            ProveedorId = "64b000000000000000000002",
            Stock = 25,
            Precio = 24.99m,
            Imagen = "imagenes/batman.png"
        };
    }
}
