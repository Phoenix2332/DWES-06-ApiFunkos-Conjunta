using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Services;

public interface ICatalogoService {
    // Categorías
    Task<Result<(IReadOnlyList<CategoriaDto> Categorias, int Total), DomainErrors>> GetAllCategoriasAsync(
        string? nombre, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    Task<Result<CategoriaDto, DomainErrors>> GetCategoriaByIdAsync(string id,
        CancellationToken cancellationToken = default);

    Task<Result<CategoriaDto, DomainErrors>> CreateCategoriaAsync(CrearCategoriaDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<CategoriaDto, DomainErrors>> UpdateCategoriaAsync(string id, ActualizarCategoriaDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> DeleteCategoriaAsync(string id, CancellationToken cancellationToken = default);


    // Proveedores
    Task<Result<(IReadOnlyList<ProveedorDto> Proveedores, int Total), DomainErrors>> GetAllProveedoresAsync(
        string? nombre, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    Task<Result<ProveedorDto, DomainErrors>> GetProveedorByIdAsync(ObjectId id,
        CancellationToken cancellationToken = default);

    Task<Result<ProveedorDto, DomainErrors>> CreateProveedorAsync(CrearProveedorDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<ProveedorDto, DomainErrors>> UpdateProveedorAsync(ObjectId id, ActualizarProveedorDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> DeleteProveedorAsync(ObjectId id, CancellationToken cancellationToken = default);


    // Funkos
    Task<Result<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>> GetAllFunkosAsync(string? nombre,
        int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    Task<Result<FunkoDto, DomainErrors>> GetFunkoByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<FunkoDto, DomainErrors>> CreateFunkoAsync(CrearFunkoDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<FunkoDto, DomainErrors>> UpdateFunkoAsync(int id, ActualizarFunkoDto dto,
        CancellationToken cancellationToken = default);

    Task<Result<bool, DomainErrors>> DeleteFunkoAsync(int id, CancellationToken cancellationToken = default);
}