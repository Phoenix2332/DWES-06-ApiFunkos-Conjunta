using System.Text.Json;
using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Services.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Services;

public class CatalogoService(
    ICategoriaService categoriaService,
    IProveedorService proveedorService,
    IFunkoService funkoService,
    IDistributedCache cache) : ICatalogoService {
    private const string _funkosCacheVersionKey = "funkos:version";
    private const string _categoriasCacheVersionKey = "categorias:version";
    private const string _proveedoresCacheVersionKey = "proveedores:version";

    // Categorias
    public async Task<Result<(IReadOnlyList<CategoriaDto> Categorias, int Total), DomainErrors>> GetAllCategoriasAsync(
        string? nombre, int pagina, int tamanoPagina, CancellationToken cancellationToken = default) {
        var version = await cache.GetStringAsync(_categoriasCacheVersionKey, cancellationToken) ?? "1";

        var cacheKey = $"categorias:{version}:{nombre?.ToLowerInvariant() ?? "todas"}:{pagina}:{tamanoPagina}";

        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedData is not null) {
            var resultadoCache = JsonSerializer.Deserialize<CacheCategoriaDto>(cachedData);

            if (resultadoCache is not null)
                return Result.Success<(IReadOnlyList<CategoriaDto> Categorias, int Total), DomainErrors>((
                    resultadoCache.Categorias, resultadoCache.Total));
        }

        var resultado = await categoriaService.GetAllAsync(nombre, pagina, tamanoPagina, cancellationToken);

        if (resultado.IsFailure)
            return Result.Failure<(IReadOnlyList<CategoriaDto> Categorias, int Total), DomainErrors>(resultado.Error);

        var datosCache = new CacheCategoriaDto(resultado.Value.Categorias, resultado.Value.Total);

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(datosCache),
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);

        return resultado;
    }

    public async Task<Result<CategoriaDto, DomainErrors>> GetCategoriaByIdAsync(string id,
        CancellationToken cancellationToken = default) {
        var cacheKey = $"categoria:{id}";

        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedData is not null) {
            var resultadoCache = JsonSerializer.Deserialize<CategoriaDto>(cachedData);

            if (resultadoCache is not null)
                return Result.Success<CategoriaDto, DomainErrors>(resultadoCache);
        }

        var resultado = await categoriaService.GetByIdAsync(id, cancellationToken);

        if (resultado.IsFailure)
            return Result.Failure<CategoriaDto, DomainErrors>(resultado.Error);

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(resultado.Value),
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);

        return resultado;
    }

    public async Task<Result<CategoriaDto, DomainErrors>> CreateCategoriaAsync(CrearCategoriaDto dto,
        CancellationToken cancellationToken = default) {
        var resultado = await categoriaService.CreateAsync(dto, cancellationToken);

        if (resultado.IsSuccess)
            await InvalidarCacheCategoriasAsync(cancellationToken: cancellationToken);

        return resultado;
    }

    public async Task<Result<CategoriaDto, DomainErrors>> UpdateCategoriaAsync(string id, ActualizarCategoriaDto dto,
        CancellationToken cancellationToken = default) {
        var resultado = await categoriaService.UpdateAsync(id, dto, cancellationToken);

        if (resultado.IsSuccess)
            await InvalidarCacheCategoriasAsync(id, cancellationToken);

        return resultado;
    }

    public async Task<Result<bool, DomainErrors>> DeleteCategoriaAsync(string id,
        CancellationToken cancellationToken = default) {
        var existenFunkos = await funkoService.ExistsByCategoriaIdAsync(id, cancellationToken);

        if (existenFunkos.IsFailure)
            return Result.Failure<bool, DomainErrors>(existenFunkos.Error);

        if (existenFunkos.Value)
            return Result.Failure<bool, DomainErrors>(CatalogoServiceError.CategoriaConFunkosError(id));

        var resultado = await categoriaService.DeleteAsync(id, cancellationToken);

        if (resultado.IsSuccess)
            await InvalidarCacheCategoriasAsync(id, cancellationToken);

        return resultado;
    }

    // Proveedores
    public async Task<Result<(IReadOnlyList<ProveedorDto> Proveedores, int Total), DomainErrors>>
        GetAllProveedoresAsync(string? nombre, int pagina, int tamanoPagina,
            CancellationToken cancellationToken = default) {
        var version = await cache.GetStringAsync(_proveedoresCacheVersionKey, cancellationToken) ?? "1";

        var cacheKey = $"proveedores:{version}:{nombre?.ToLowerInvariant() ?? "todos"}:{pagina}:{tamanoPagina}";

        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedData is not null) {
            var resultadoCache = JsonSerializer.Deserialize<CacheProveedorDto>(cachedData);

            if (resultadoCache is not null)
                return Result.Success<(IReadOnlyList<ProveedorDto> Proveedores, int Total), DomainErrors>((
                    resultadoCache.Proveedores, resultadoCache.Total));
        }

        var resultado = await proveedorService.GetAllAsync(nombre, pagina, tamanoPagina, cancellationToken);

        if (resultado.IsFailure)
            return Result.Failure<(IReadOnlyList<ProveedorDto> Proveedores, int Total), DomainErrors>(resultado.Error);

        var datosCache = new CacheProveedorDto(resultado.Value.Proveedores, resultado.Value.Total);

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(datosCache),
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);

        return resultado;
    }

    public async Task<Result<ProveedorDto, DomainErrors>> GetProveedorByIdAsync(ObjectId id,
        CancellationToken cancellationToken = default) {
        var cacheKey = $"proveedor:{id}";

        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedData is not null) {
            var resultadoCache = JsonSerializer.Deserialize<ProveedorDto>(cachedData);

            if (resultadoCache is not null)
                return Result.Success<ProveedorDto, DomainErrors>(resultadoCache);
        }

        var resultado = await proveedorService.GetByIdAsync(id, cancellationToken);

        if (resultado.IsFailure)
            return Result.Failure<ProveedorDto, DomainErrors>(resultado.Error);

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(resultado.Value),
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);

        return resultado;
    }

    public async Task<Result<ProveedorDto, DomainErrors>> CreateProveedorAsync(CrearProveedorDto dto,
        CancellationToken cancellationToken = default) {
        var resultado = await proveedorService.CreateAsync(dto, cancellationToken);

        if (resultado.IsSuccess)
            await InvalidarCacheProveedoresAsync(cancellationToken: cancellationToken);

        return resultado;
    }

    public async Task<Result<ProveedorDto, DomainErrors>> UpdateProveedorAsync(ObjectId id, ActualizarProveedorDto dto,
        CancellationToken cancellationToken = default) {
        var resultado = await proveedorService.UpdateAsync(id, dto, cancellationToken);

        if (resultado.IsSuccess)
            await InvalidarCacheProveedoresAsync(id, cancellationToken);

        return resultado;
    }

    public async Task<Result<bool, DomainErrors>> DeleteProveedorAsync(ObjectId id,
        CancellationToken cancellationToken = default) {
        var existenFunkos = await funkoService.ExistsByProveedorIdAsync(id.ToString(), cancellationToken);

        if (existenFunkos.IsFailure)
            return Result.Failure<bool, DomainErrors>(existenFunkos.Error);

        if (existenFunkos.Value)
            return Result.Failure<bool, DomainErrors>(CatalogoServiceError.ProveedorConFunkosError(id.ToString()));

        var resultado = await proveedorService.DeleteAsync(id, cancellationToken);

        if (resultado.IsSuccess)
            await InvalidarCacheProveedoresAsync(id, cancellationToken);

        return resultado;
    }

    // Funkos
    public async Task<Result<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>> GetAllFunkosAsync(
        string? nombre, int pagina, int tamanoPagina, CancellationToken cancellationToken = default) {
        var version = await cache.GetStringAsync(_funkosCacheVersionKey, cancellationToken) ?? "1";

        var cacheKey = $"funkos:{version}:{nombre?.ToLowerInvariant() ?? "todos"}:{pagina}:{tamanoPagina}";

        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedData is not null) {
            var resultadoCache = JsonSerializer.Deserialize<CacheFunkoDto>(cachedData);

            if (resultadoCache is not null)
                return Result.Success<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>((resultadoCache.Funkos,
                    resultadoCache.Total));
        }

        var resultado = await funkoService.GetAllAsync(nombre, pagina, tamanoPagina, cancellationToken);

        if (resultado.IsFailure)
            return Result.Failure<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>(resultado.Error);

        var datosCache = new CacheFunkoDto(resultado.Value.Funkos, resultado.Value.Total);

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(datosCache),
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);

        return resultado;
    }

    public async Task<Result<FunkoDto, DomainErrors>> GetFunkoByIdAsync(int id,
        CancellationToken cancellationToken = default) {
        var cacheKey = $"funko:{id}";

        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedData is not null) {
            var resultadoCache = JsonSerializer.Deserialize<FunkoDto>(cachedData);

            if (resultadoCache is not null)
                return Result.Success<FunkoDto, DomainErrors>(resultadoCache);
        }

        var resultado = await funkoService.GetByIdAsync(id, cancellationToken);

        if (resultado.IsFailure)
            return Result.Failure<FunkoDto, DomainErrors>(resultado.Error);

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(resultado.Value),
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);

        return resultado;
    }

    public async Task<Result<FunkoDto, DomainErrors>> CreateFunkoAsync(CrearFunkoDto dto,
        CancellationToken cancellationToken = default) {
        var categoria = await categoriaService.GetByIdAsync(dto.CategoriaId, cancellationToken);

        if (categoria.IsFailure)
            return Result.Failure<FunkoDto, DomainErrors>(categoria.Error);

        if (!ObjectId.TryParse(dto.ProveedorId, out var proveedorId))
            return Result.Failure<FunkoDto, DomainErrors>(
                CatalogoServiceError.ProveedorIdInvalidoError(dto.ProveedorId));

        var proveedor = await proveedorService.GetByIdAsync(
            proveedorId, cancellationToken);

        if (proveedor.IsFailure)
            return Result.Failure<FunkoDto, DomainErrors>(proveedor.Error);

        var resultado = await funkoService.CreateAsync(dto, cancellationToken);

        if (resultado.IsSuccess)
            await InvalidarCacheFunkosAsync(cancellationToken: cancellationToken);

        return resultado;
    }

    public async Task<Result<FunkoDto, DomainErrors>> UpdateFunkoAsync(int id, ActualizarFunkoDto dto,
        CancellationToken cancellationToken = default) {
        var categoria = await categoriaService.GetByIdAsync(dto.CategoriaId, cancellationToken);

        if (categoria.IsFailure)
            return Result.Failure<FunkoDto, DomainErrors>(categoria.Error);

        if (!ObjectId.TryParse(dto.ProveedorId, out var proveedorId))
            return Result.Failure<FunkoDto, DomainErrors>(
                CatalogoServiceError.ProveedorIdInvalidoError(dto.ProveedorId));

        var proveedor = await proveedorService.GetByIdAsync(
            proveedorId, cancellationToken);

        if (proveedor.IsFailure)
            return Result.Failure<FunkoDto, DomainErrors>(proveedor.Error);

        var resultado = await funkoService.UpdateAsync(id, dto, cancellationToken);

        if (resultado.IsSuccess)
            await InvalidarCacheFunkosAsync(id, cancellationToken);

        return resultado;
    }

    public async Task<Result<bool, DomainErrors>> DeleteFunkoAsync(int id,
        CancellationToken cancellationToken = default) {
        var resultado = await funkoService.DeleteAsync(id, cancellationToken);

        if (resultado.IsSuccess)
            await InvalidarCacheFunkosAsync(id, cancellationToken);

        return resultado;
    }

    private async Task InvalidarCacheProveedoresAsync(ObjectId? id = null,
        CancellationToken cancellationToken = default) {
        await cache.SetStringAsync(_proveedoresCacheVersionKey, Guid.NewGuid().ToString(), cancellationToken);

        if (id.HasValue)
            await cache.RemoveAsync($"proveedor:{id.Value}", cancellationToken);
    }

    private async Task InvalidarCacheCategoriasAsync(string? id = null, CancellationToken cancellationToken = default) {
        await cache.SetStringAsync(_categoriasCacheVersionKey, Guid.NewGuid().ToString(), cancellationToken);

        if (id is not null)
            await cache.RemoveAsync($"categoria:{id}", cancellationToken);
    }

    private async Task InvalidarCacheFunkosAsync(int? id = null, CancellationToken cancellationToken = default) {
        await cache.SetStringAsync(_funkosCacheVersionKey, Guid.NewGuid().ToString(), cancellationToken);

        if (id.HasValue)
            await cache.RemoveAsync($"funko:{id.Value}", cancellationToken);
    }
}