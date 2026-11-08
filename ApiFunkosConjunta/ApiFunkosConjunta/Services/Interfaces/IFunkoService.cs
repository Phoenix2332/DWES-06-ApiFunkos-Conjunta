using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;

namespace ApiFunkosConjunta.Services.Interfaces;

public interface IFunkoService {
    Task<Result<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>> GetAllAsync(string? nombre, int pagina,
        int tamanoPagina, CancellationToken cancellationToken = default);

    Task<Result<FunkoDto, DomainErrors>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<FunkoDto, DomainErrors>> CreateAsync(CrearFunkoDto dto, int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Result<FunkoDto, DomainErrors>> UpdateAsync(int id, ActualizarFunkoDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> ExistsByCategoriaIdAsync(string categoriaId,
        CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> ExistsByProveedorIdAsync(string proveedorId,
        CancellationToken cancellationToken = default);
}