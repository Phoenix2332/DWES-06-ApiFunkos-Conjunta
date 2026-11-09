using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiFunkosConjunta.Dto.Proveedores;

/// <summary>
/// DTO con los datos necesarios para crear un proveedor en el catalogo.
/// </summary>
[SwaggerSchema(Description = "Datos necesarios para crear un proveedor en el catálogo.")]
public class CrearProveedorDto {
    /// <summary>
    /// Nombre comercial del proveedor.
    /// </summary>
    /// <example>Funko</example>
    [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El nombre no puede contener únicamente espacios.")]
    [SwaggerSchema(Description = "Nombre comercial del proveedor.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Datos de contacto del proveedor (correo electronico, telefono o web).
    /// </summary>
    /// <example>ventas@funko.com</example>
    [Required(ErrorMessage = "El contacto del proveedor es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El contacto debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El contacto no puede contener únicamente espacios.")]
    [SwaggerSchema(Description = "Datos de contacto del proveedor.")]
    public string Contacto { get; set; } = string.Empty;

    /// <summary>
    /// Pais donde esta situado el proveedor.
    /// </summary>
    /// <example>Estados Unidos</example>
    [Required(ErrorMessage = "El país del proveedor es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El país debe tener entre 2 y 50 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El país no puede contener únicamente espacios.")]
    [SwaggerSchema(Description = "País donde está situado el proveedor.")]
    public string Pais { get; set; } = string.Empty;
}
