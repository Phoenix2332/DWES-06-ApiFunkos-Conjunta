using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using CSharpFunctionalExtensions;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Repositories.Interfaces;

public interface IProveedorRepository {
    Task<Result<(IReadOnlyList<Proveedor> Proveedores, int Total), DomainErrors>> GetAllAsync(string? nombre,
        int pagina, int tamanoPagina,
        CancellationToken cancellationToken = default);

    Task<Result<Proveedor?, DomainErrors>> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default);

    Task<Result<Proveedor, DomainErrors>> AddAsync(Proveedor proveedor, CancellationToken cancellationToken = default);

    Task<Result<Proveedor, DomainErrors>> UpdateAsync(Proveedor proveedor,
        CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> DeleteAsync(Proveedor proveedor, CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> ExistsByNameAsync(string nombre, ObjectId? idExcluir = null,
        CancellationToken cancellationToken = default);
}
