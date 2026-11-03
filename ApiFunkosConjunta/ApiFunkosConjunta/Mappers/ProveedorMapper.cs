using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Models;

namespace ApiFunkosConjunta.Mappers;

public static class ProveedorMapper {
    public static Proveedor ToModel(CrearProveedorDto dto) {
        return new Proveedor {
            Nombre = dto.Nombre.Trim(),
            Contacto = dto.Contacto.Trim(),
            Pais = dto.Pais.Trim()
        };
    }

    public static ProveedorDto ToDto(Proveedor proveedor) {
        return new ProveedorDto {
            Id = proveedor.Id,
            Nombre = proveedor.Nombre,
            Contacto = proveedor.Contacto,
            Pais = proveedor.Pais
        };
    }

    public static void UpdateModel(Proveedor proveedor, ActualizarProveedorDto dto) {
        proveedor.Nombre = dto.Nombre.Trim();
        proveedor.Contacto = dto.Contacto.Trim();
        proveedor.Pais = dto.Pais.Trim();
    }
}
