using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Mappers;
using ApiFunkosConjunta.Repositories.Interfaces;
using ApiFunkosConjunta.Services.Interfaces;
using CSharpFunctionalExtensions;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Services.Implementations;

public class ProveedorService(IProveedorRepository repository) : IProveedorService {
    public async Task<Result<(IReadOnlyList<ProveedorDto> Proveedores, int Total), DomainErrors>> GetAllAsync(
        string? nombre, int pagina, int tamanoPagina, CancellationToken cancellationToken = default) {
        var result = await repository.GetAllAsync(nombre, pagina, tamanoPagina, cancellationToken);

        if (result.IsFailure)
            return Result.Failure<(IReadOnlyList<ProveedorDto>, int), DomainErrors>(result.Error);

        var proveedores = result.Value.Proveedores
            .Select(ProveedorMapper.ToDto)
            .ToList();

        return Result.Success<(IReadOnlyList<ProveedorDto>, int), DomainErrors>((proveedores, result.Value.Total));
    }

    public async Task<Result<ProveedorDto, DomainErrors>> GetByIdAsync(ObjectId id,
        CancellationToken cancellationToken = default) {
        var result = await repository.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
            return Result.Failure<ProveedorDto, DomainErrors>(result.Error);

        return result.Value is null
            ? Result.Failure<ProveedorDto, DomainErrors>(ProveedorServiceError.NotFoundError(id.ToString()))
            : Result.Success<ProveedorDto, DomainErrors>(ProveedorMapper.ToDto(result.Value));
    }

    public async Task<Result<ProveedorDto, DomainErrors>> CreateAsync(CrearProveedorDto dto,
        CancellationToken cancellationToken = default) {
        var nombre = dto.Nombre.Trim();

        var existsResult = await repository.ExistsByNameAsync(nombre, cancellationToken: cancellationToken);

        if (existsResult.IsFailure)
            return Result.Failure<ProveedorDto, DomainErrors>(existsResult.Error);

        if (existsResult.Value)
            return Result.Failure<ProveedorDto, DomainErrors>(ProveedorServiceError.DuplicateNameError(nombre));

        var proveedor = ProveedorMapper.ToModel(dto);

        var result = await repository.AddAsync(proveedor, cancellationToken);

        return result.IsFailure
            ? Result.Failure<ProveedorDto, DomainErrors>(result.Error)
            : Result.Success<ProveedorDto, DomainErrors>(ProveedorMapper.ToDto(result.Value));
    }

    public async Task<Result<ProveedorDto, DomainErrors>> UpdateAsync(ObjectId id, ActualizarProveedorDto dto,
        CancellationToken cancellationToken = default) {
        var getResult = await repository.GetByIdAsync(id, cancellationToken);

        if (getResult.IsFailure)
            return Result.Failure<ProveedorDto, DomainErrors>(getResult.Error);

        if (getResult.Value is null)
            return Result.Failure<ProveedorDto, DomainErrors>(ProveedorServiceError.NotFoundError(id.ToString()));

        var nombre = dto.Nombre.Trim();

        var existsResult = await repository.ExistsByNameAsync(nombre, id, cancellationToken);

        if (existsResult.IsFailure)
            return Result.Failure<ProveedorDto, DomainErrors>(existsResult.Error);

        if (existsResult.Value)
            return Result.Failure<ProveedorDto, DomainErrors>(ProveedorServiceError.DuplicateNameError(nombre));

        var proveedor = getResult.Value;

        ProveedorMapper.UpdateModel(proveedor, dto);

        var updateResult = await repository.UpdateAsync(proveedor, cancellationToken);

        return updateResult.IsFailure
            ? Result.Failure<ProveedorDto, DomainErrors>(updateResult.Error)
            : Result.Success<ProveedorDto, DomainErrors>(ProveedorMapper.ToDto(updateResult.Value));
    }

    public async Task<Result<bool, DomainErrors>>
        DeleteAsync(ObjectId id, CancellationToken cancellationToken = default) {
        var getResult = await repository.GetByIdAsync(id, cancellationToken);

        if (getResult.IsFailure)
            return Result.Failure<bool, DomainErrors>(getResult.Error);

        if (getResult.Value is null)
            return Result.Failure<bool, DomainErrors>(ProveedorServiceError.NotFoundError(id.ToString()));

        return await repository.DeleteAsync(getResult.Value, cancellationToken);
    }
}
