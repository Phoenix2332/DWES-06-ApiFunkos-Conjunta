using Swashbuckle.AspNetCore.Annotations;

namespace ApiFunkosConjunta.Dto.Categorias;

/// <summary>
/// DTO con los datos publicos de una categoria del catalogo.
/// </summary>
[SwaggerSchema(Description = "Datos públicos de una categoría del catálogo.")]
public record CategoriaDto {
    /// <summary>
    /// Identificador de la categoria.
    /// </summary>
    /// <example>D2M_Lk6kKe6</example>
    [SwaggerSchema(Description = "Identificador de la categoría.")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Nombre de la categoria.
    /// </summary>
    /// <example>Videojuegos</example>
    [SwaggerSchema(Description = "Nombre de la categoría.")]
    public string Nombre { get; set; } = string.Empty;
}
