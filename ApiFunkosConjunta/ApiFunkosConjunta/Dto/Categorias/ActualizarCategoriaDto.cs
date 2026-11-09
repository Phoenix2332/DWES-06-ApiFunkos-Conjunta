using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiFunkosConjunta.Dto.Categorias;

/// <summary>
/// DTO con los datos necesarios para actualizar una categoria del catalogo.
/// </summary>
[SwaggerSchema(Description = "Datos necesarios para actualizar una categoría del catálogo.")]
public class ActualizarCategoriaDto {
    /// <summary>
    /// Nombre de la categoria.
    /// </summary>
    /// <example>Figuras</example>
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El nombre no puede contener únicamente espacios.")]
    [SwaggerSchema(Description = "Nombre de la categoría.")]
    public string Nombre { get; set; } = string.Empty;
}
