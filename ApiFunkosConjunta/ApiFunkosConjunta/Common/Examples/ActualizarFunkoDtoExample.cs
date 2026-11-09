using ApiFunkosConjunta.Dto.Funkos;
using Swashbuckle.AspNetCore.Filters;

namespace ApiFunkosConjunta.Common.Examples;

/// <summary>
/// Ejemplo de peticion para actualizar un Funko (apartado 24.8 del tema).
/// </summary>
public class ActualizarFunkoDtoExample : IExamplesProvider<ActualizarFunkoDto> {
    /// <summary>
    /// Devuelve un Funko de ejemplo que Swagger UI muestra como cuerpo de la peticion.
    /// </summary>
    public ActualizarFunkoDto GetExamples() {
        return new ActualizarFunkoDto {
            Nombre = "Funko Pop! Batman (edicion 2026)",
            CategoriaId = "D2M_Lk6kKe6",
            ProveedorId = "64b000000000000000000002",
            Stock = 12,
            Precio = 29.99m,
            Imagen = "imagenes/batman-2026.png"
        };
    }
}
