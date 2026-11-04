using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using CSharpFunctionalExtensions;

namespace ApiFunkosConjunta.Repositories.Interfaces;

public interface IFunkoRepository {
    Task<Result<(IReadOnlyList<Funko> Funkos, int Total), DomainErrors>> GetAllAsync(string? nombre, int pagina,
        int tamanoPagina, CancellationToken cancellationToken = default);

    Task<Result<Funko?, DomainErrors>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<Funko, DomainErrors>> AddAsync(Funko funko, CancellationToken cancellationToken = default);

    Task<Result<Funko, DomainErrors>> UpdateAsync(Funko funko, CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> DeleteAsync(Funko funko, CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> ExistsByCategoriaIdAsync(string categoriaId,
        CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> ExistsByProveedorIdAsync(string proveedorId,
        CancellationToken cancellationToken = default);
}