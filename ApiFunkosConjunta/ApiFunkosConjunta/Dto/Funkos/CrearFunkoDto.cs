using System.ComponentModel.DataAnnotations;

namespace ApiFunkosConjunta.Dto.Funkos;

public class CrearFunkoDto {
    [Required(ErrorMessage = "El nombre del Funko es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El nombre no puede contener únicamente espacios.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "El identificador de la categoría debe tener 11 caracteres.")]
    public string CategoriaId { get; set; } = string.Empty;

    [Required(ErrorMessage = "El proveedor es obligatorio.")]
    [StringLength(24, MinimumLength = 24, ErrorMessage = "El identificador del proveedor debe tener 24 caracteres.")]
    [RegularExpression(@"^[a-fA-F0-9]{24}$",
        ErrorMessage = "El identificador del proveedor no tiene un formato válido.")]
    public string ProveedorId { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    // Ojo: con la forma [Range(typeof(decimal), "0.01", "...")] el atributo parsea los limites con la
    // cultura actual y falla con cultures que usan la coma decimal (p. ej. es-ES): FormatException.
    // La forma con double convierte los limites a decimal sin depender de la cultura.
    [Range(0.01, 99999999.99, ErrorMessage = "El precio debe ser mayor que 0.")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La imagen es obligatoria.")]
    [StringLength(255, ErrorMessage = "La ruta de la imagen no puede superar los 255 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "La ruta de la imagen no puede contener únicamente espacios.")]
    public string Imagen { get; set; } = string.Empty;
}