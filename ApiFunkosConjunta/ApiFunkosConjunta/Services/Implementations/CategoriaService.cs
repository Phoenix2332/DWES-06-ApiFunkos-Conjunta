using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Mappers;
using ApiFunkosConjunta.Repositories.Interfaces;
using ApiFunkosConjunta.Services.Interfaces;
using CSharpFunctionalExtensions;

namespace ApiFunkosConjunta.Services.Implementations;

public class CategoriaService(ICategoriaRepository repository) : ICategoriaService {
    public async Task<Result<(IReadOnlyList<CategoriaDto> Categorias, int Total), DomainErrors>> GetAllAsync(
        string? nombre, int pagina, int tamanoPagina, CancellationToken cancellationToken = default) {
        var result = await repository.GetAllAsync(nombre, pagina, tamanoPagina, cancellationToken);

        if (result.IsFailure)
            return Result.Failure<(IReadOnlyList<CategoriaDto>, int), DomainErrors>(result.Error);

        var categorias = result.Value.Categorias
            .Select(CategoriaMapper.ToDto)
            .ToList();

        return Result.Success<(IReadOnlyList<CategoriaDto>, int), DomainErrors>((categorias, result.Value.Total));
    }

    public async Task<Result<CategoriaDto, DomainErrors>> GetByIdAsync(string id,
        CancellationToken cancellationToken = default) {
        var result = await repository.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
            return Result.Failure<CategoriaDto, DomainErrors>(result.Error);

        return result.Value is null
            ? Result.Failure<CategoriaDto, DomainErrors>(ServiceError.NotFoundError(id))
            : Result.Success<CategoriaDto, DomainErrors>(CategoriaMapper.ToDto(result.Value));
    }

    public async Task<Result<CategoriaDto, DomainErrors>> CreateAsync(CrearCategoriaDto dto,
        CancellationToken cancellationToken = default) {
        var nombre = dto.Nombre.Trim();

        var existsResult = await repository.ExistsByNameAsync(nombre, cancellationToken: cancellationToken);

        if (existsResult.IsFailure)
            return Result.Failure<CategoriaDto, DomainErrors>(existsResult.Error);

        if (existsResult.Value)
            return Result.Failure<CategoriaDto, DomainErrors>(ServiceError.DuplicateNameError(nombre));

        var categoria = CategoriaMapper.ToModel(dto);

        var result = await repository.AddAsync(categoria, cancellationToken);

        return result.IsFailure
            ? Result.Failure<CategoriaDto, DomainErrors>(result.Error)
            : Result.Success<CategoriaDto, DomainErrors>(CategoriaMapper.ToDto(result.Value));
    }

    public async Task<Result<CategoriaDto, DomainErrors>> UpdateAsync(string id, ActualizarCategoriaDto dto,
        CancellationToken cancellationToken = default) {
        var getResult = await repository.GetByIdAsync(id, cancellationToken);

        if (getResult.IsFailure)
            return Result.Failure<CategoriaDto, DomainErrors>(getResult.Error);

        if (getResult.Value is null)
            return Result.Failure<CategoriaDto, DomainErrors>(ServiceError.NotFoundError(id));

        var nombre = dto.Nombre.Trim();

        var existsResult = await repository.ExistsByNameAsync(nombre, id, cancellationToken);

        if (existsResult.IsFailure)
            return Result.Failure<CategoriaDto, DomainErrors>(existsResult.Error);

        if (existsResult.Value)
            return Result.Failure<CategoriaDto, DomainErrors>(ServiceError.DuplicateNameError(nombre));

        var categoria = getResult.Value;

        CategoriaMapper.UpdateModel(categoria, dto);

        var updateResult = await repository.UpdateAsync(categoria, cancellationToken);

        return updateResult.IsFailure
            ? Result.Failure<CategoriaDto, DomainErrors>(updateResult.Error)
            : Result.Success<CategoriaDto, DomainErrors>(CategoriaMapper.ToDto(updateResult.Value));
    }

    public async Task<Result<bool, DomainErrors>>
        DeleteAsync(string id, CancellationToken cancellationToken = default) {
        var getResult = await repository.GetByIdAsync(id, cancellationToken);

        if (getResult.IsFailure)
            return Result.Failure<bool, DomainErrors>(getResult.Error);

        if (getResult.Value is null)
            return Result.Failure<bool, DomainErrors>(ServiceError.NotFoundError(id));

        return await repository.DeleteAsync(getResult.Value, cancellationToken);
    }
}