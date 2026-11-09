using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiFunkosConjunta.Dto.Funkos;

/// <summary>
/// DTO con los datos necesarios para actualizar un Funko del catalogo.
/// </summary>
[SwaggerSchema(Description = "Datos necesarios para actualizar un Funko del catálogo.")]
public class ActualizarFunkoDto {
    /// <summary>
    /// Nombre comercial del Funko.
    /// </summary>
    /// <example>Funko Pop! Batman (edicion 2026)</example>
    [Required(ErrorMessage = "El nombre del Funko es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El nombre no puede contener únicamente espacios.")]
    [SwaggerSchema(Description = "Nombre comercial del Funko.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Identificador de la categoria a la que pertenece el Funko.
    /// </summary>
    /// <example>D2M_Lk6kKe6</example>
    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "El identificador de la categoría debe tener 11 caracteres.")]
    [SwaggerSchema(Description = "Identificador de la categoría del Funko.")]
    public string CategoriaId { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del proveedor del Funko (ObjectId de MongoDB).
    /// </summary>
    /// <example>64b000000000000000000002</example>
    [Required(ErrorMessage = "El proveedor es obligatorio.")]
    [StringLength(24, MinimumLength = 24, ErrorMessage = "El identificador del proveedor debe tener 24 caracteres.")]
    [RegularExpression(@"^[a-fA-F0-9]{24}$",
        ErrorMessage = "El identificador del proveedor no tiene un formato válido.")]
    [SwaggerSchema(Description = "Identificador del proveedor del Funko.")]
    public string ProveedorId { get; set; } = string.Empty;

    /// <summary>
    /// Unidades disponibles del Funko en el catalogo.
    /// </summary>
    /// <example>12</example>
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    [SwaggerSchema(Description = "Unidades disponibles del Funko en el catálogo.")]
    public int Stock { get; set; }

    /// <summary>
    /// Precio del Funko en euros.
    /// </summary>
    /// <example>29.99</example>
    // Ojo: con la forma [Range(typeof(decimal), "0.01", "...")] el atributo parsea los limites con la
    // cultura actual y falla con cultures que usan la coma decimal (p. ej. es-ES): FormatException.
    // La forma con double convierte los limites a decimal sin depender de la cultura.
    [Range(0.01, 99999999.99, ErrorMessage = "El precio debe ser mayor que 0.")]
    [SwaggerSchema(Description = "Precio del Funko en euros.", Format = "decimal")]
    public decimal Precio { get; set; }

    /// <summary>
    /// Ruta de la imagen del Funko.
    /// </summary>
    /// <example>imagenes/batman-2026.png</example>
    [Required(ErrorMessage = "La imagen es obligatoria.")]
    [StringLength(255, ErrorMessage = "La ruta de la imagen no puede superar los 255 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "La ruta de la imagen no puede contener únicamente espacios.")]
    [SwaggerSchema(Description = "Ruta de la imagen del Funko.")]
    public string Imagen { get; set; } = string.Empty;
}
