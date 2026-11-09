using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiFunkosConjunta.Dto.Proveedores;

/// <summary>
/// DTO con los datos publicos de un proveedor del catalogo.
/// </summary>
[SwaggerSchema(Description = "Datos públicos de un proveedor del catálogo.")]
public record ProveedorDto {
    /// <summary>
    /// Identificador del proveedor (ObjectId de MongoDB).
    /// </summary>
    /// <example>64b000000000000000000002</example>
    [SwaggerSchema(Description = "Identificador del proveedor (ObjectId de MongoDB).")]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Nombre comercial del proveedor.
    /// </summary>
    /// <example>Funko</example>
    [SwaggerSchema(Description = "Nombre comercial del proveedor.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Datos de contacto del proveedor (correo electronico, telefono o web).
    /// </summary>
    /// <example>ventas@funko.com</example>
    [SwaggerSchema(Description = "Datos de contacto del proveedor.")]
    public string Contacto { get; set; } = string.Empty;

    /// <summary>
    /// Pais donde esta situado el proveedor.
    /// </summary>
    /// <example>Estados Unidos</example>
    [SwaggerSchema(Description = "País donde está situado el proveedor.")]
    public string Pais { get; set; } = string.Empty;
}
