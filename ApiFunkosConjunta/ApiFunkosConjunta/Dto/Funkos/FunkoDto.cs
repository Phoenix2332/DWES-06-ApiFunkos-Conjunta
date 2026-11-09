using Swashbuckle.AspNetCore.Annotations;

namespace ApiFunkosConjunta.Dto.Funkos;

/// <summary>
/// DTO con los datos publicos de un Funko del catalogo.
/// </summary>
[SwaggerSchema(Description = "Datos públicos de un Funko del catálogo.")]
public record FunkoDto {
    /// <summary>
    /// Identificador del Funko, generado por la base de datos.
    /// </summary>
    /// <example>1</example>
    [SwaggerSchema(Description = "Identificador del Funko.", Format = "int32")]
    public int Id { get; set; }

    /// <summary>
    /// Nombre comercial del Funko.
    /// </summary>
    /// <example>Funko Pop! Batman</example>
    [SwaggerSchema(Description = "Nombre comercial del Funko.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Identificador de la categoria a la que pertenece el Funko.
    /// </summary>
    /// <example>D2M_Lk6kKe6</example>
    [SwaggerSchema(Description = "Identificador de la categoría del Funko.")]
    public string CategoriaId { get; set; } = null!;

    /// <summary>
    /// Identificador del proveedor del Funko.
    /// </summary>
    /// <example>64b000000000000000000002</example>
    [SwaggerSchema(Description = "Identificador del proveedor del Funko.")]
    public string ProveedorId { get; set; } = null!;

    /// <summary>
    /// Unidades disponibles del Funko en el catalogo.
    /// </summary>
    /// <example>25</example>
    [SwaggerSchema(Description = "Unidades disponibles del Funko en el catálogo.", Format = "int32")]
    public int Stock { get; set; }

    /// <summary>
    /// Precio del Funko en euros.
    /// </summary>
    /// <example>24.99</example>
    [SwaggerSchema(Description = "Precio del Funko en euros.", Format = "decimal")]
    public decimal Precio { get; set; }

    /// <summary>
    /// Ruta de la imagen del Funko.
    /// </summary>
    /// <example>imagenes/batman.png</example>
    [SwaggerSchema(Description = "Ruta de la imagen del Funko.")]
    public string Imagen { get; set; } = string.Empty;
}
