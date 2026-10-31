using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using CSharpFunctionalExtensions;

namespace ApiFunkosConjunta.Repositories.Interfaces;

public interface ICategoriaRepository {
    Task<Result<(IReadOnlyList<Categoria> Categorias, int Total), DomainErrors>> GetAllAsync(string? nombre, int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default);

    Task<Result<Categoria?, DomainErrors>> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<Result<Categoria, DomainErrors>> AddAsync(Categoria categoria, CancellationToken cancellationToken = default);

    Task<Result<Categoria, DomainErrors>> UpdateAsync(Categoria categoria,
        CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> DeleteAsync(Categoria categoria, CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> ExistsByNameAsync(string nombre, string? idExcluir = null,
        CancellationToken cancellationToken = default);
}