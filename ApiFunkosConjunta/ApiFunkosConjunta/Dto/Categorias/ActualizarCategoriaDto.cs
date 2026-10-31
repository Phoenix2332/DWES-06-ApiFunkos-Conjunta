using System.ComponentModel.DataAnnotations;

namespace ApiFunkosConjunta.Dto.Categorias;

public class ActualizarCategoriaDto {
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El nombre no puede contener únicamente espacios.")]
    public string Nombre { get; set; } = string.Empty;
}