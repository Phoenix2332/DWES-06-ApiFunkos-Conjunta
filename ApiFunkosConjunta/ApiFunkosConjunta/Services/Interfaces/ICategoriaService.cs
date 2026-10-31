using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;

namespace ApiFunkosConjunta.Services.Interfaces;

public interface ICategoriaService {
    Task<Result<(IReadOnlyList<CategoriaDto> Categorias, int Total), DomainErrors>> GetAllAsync(string? nombre,
        int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    Task<Result<CategoriaDto, DomainErrors>> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<Result<CategoriaDto, DomainErrors>> CreateAsync(CrearCategoriaDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<CategoriaDto, DomainErrors>> UpdateAsync(string id, ActualizarCategoriaDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> DeleteAsync(string id, CancellationToken cancellationToken = default);
}