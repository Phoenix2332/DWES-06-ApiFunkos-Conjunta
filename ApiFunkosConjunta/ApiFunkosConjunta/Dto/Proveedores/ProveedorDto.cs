using MongoDB.Bson;

namespace ApiFunkosConjunta.Dto.Proveedores;

public record ProveedorDto {
    public ObjectId Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Contacto { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
}
