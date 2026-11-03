using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Services.Interfaces;

public interface IProveedorService {
    Task<Result<(IReadOnlyList<ProveedorDto> Proveedores, int Total), DomainErrors>> GetAllAsync(string? nombre,
        int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    Task<Result<ProveedorDto, DomainErrors>> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default);

    Task<Result<ProveedorDto, DomainErrors>> CreateAsync(CrearProveedorDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<ProveedorDto, DomainErrors>> UpdateAsync(ObjectId id, ActualizarProveedorDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> DeleteAsync(ObjectId id, CancellationToken cancellationToken = default);
}
