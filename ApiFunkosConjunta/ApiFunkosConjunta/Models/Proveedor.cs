using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiFunkosConjunta.Models;

public class Proveedor {
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("contacto")]
    public string Contacto { get; set; } = string.Empty;

    [BsonElement("pais")]
    public string Pais { get; set; } = string.Empty;
}
