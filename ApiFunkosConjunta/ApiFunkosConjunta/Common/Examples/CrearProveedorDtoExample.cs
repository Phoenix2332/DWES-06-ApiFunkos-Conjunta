using ApiFunkosConjunta.Dto.Proveedores;
using Swashbuckle.AspNetCore.Filters;

namespace ApiFunkosConjunta.Common.Examples;

/// <summary>
/// Ejemplo de peticion para crear un proveedor (apartado 24.8 del tema).
/// </summary>
public class CrearProveedorDtoExample : IExamplesProvider<CrearProveedorDto> {
    /// <summary>
    /// Devuelve un proveedor de ejemplo que Swagger UI muestra como cuerpo de la peticion.
    /// </summary>
    public CrearProveedorDto GetExamples() {
        return new CrearProveedorDto {
            Nombre = "Funko",
            Contacto = "ventas@funko.com",
            Pais = "Estados Unidos"
        };
    }
}
