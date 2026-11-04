namespace ApiFunkosConjunta.Dto.Proveedores;

public record CacheProveedorDto(IReadOnlyList<ProveedorDto> Proveedores, int Total);