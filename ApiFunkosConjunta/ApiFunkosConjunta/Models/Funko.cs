namespace ApiFunkosConjunta.Models;

public class Funko {
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string CategoriaId { get; set; } = null!;

    public string ProveedorId { get; set; } = null!;

    public int Stock { get; set; }

    public decimal Precio { get; set; }

    public string Imagen { get; set; } = string.Empty;
}