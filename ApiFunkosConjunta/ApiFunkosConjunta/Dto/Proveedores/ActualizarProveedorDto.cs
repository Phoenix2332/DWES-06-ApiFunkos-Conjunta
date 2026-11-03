using System.ComponentModel.DataAnnotations;

namespace ApiFunkosConjunta.Dto.Proveedores;

public class ActualizarProveedorDto {
    [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El nombre no puede contener únicamente espacios.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contacto del proveedor es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El contacto debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El contacto no puede contener únicamente espacios.")]
    public string Contacto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El país del proveedor es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El país debe tener entre 2 y 50 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El país no puede contener únicamente espacios.")]
    public string Pais { get; set; } = string.Empty;
}
