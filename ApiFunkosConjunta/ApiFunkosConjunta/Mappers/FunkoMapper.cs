using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Models;

namespace ApiFunkosConjunta.Mappers;

public static class FunkoMapper {
    public static Funko ToModel(CrearFunkoDto dto) {
        return new Funko {
            Nombre = dto.Nombre.Trim(),
            CategoriaId = dto.CategoriaId.Trim(),
            ProveedorId = dto.ProveedorId.Trim(),
            Stock = dto.Stock,
            Precio = dto.Precio,
            Imagen = dto.Imagen.Trim()
        };
    }

    public static FunkoDto ToDto(Funko funko) {
        return new FunkoDto {
            Id = funko.Id,
            Nombre = funko.Nombre,
            CategoriaId = funko.CategoriaId,
            ProveedorId = funko.ProveedorId,
            Stock = funko.Stock,
            Precio = funko.Precio,
            Imagen = funko.Imagen
        };
    }

    public static void UpdateModel(Funko funko, ActualizarFunkoDto dto) {
        funko.Nombre = dto.Nombre.Trim();
        funko.CategoriaId = dto.CategoriaId.Trim();
        funko.ProveedorId = dto.ProveedorId.Trim();
        funko.Stock = dto.Stock;
        funko.Precio = dto.Precio;
        funko.Imagen = dto.Imagen.Trim();
    }
}