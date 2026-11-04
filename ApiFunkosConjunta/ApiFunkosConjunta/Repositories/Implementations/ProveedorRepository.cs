using System.Text.RegularExpressions;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Repositories.Interfaces;
using CSharpFunctionalExtensions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApiFunkosConjunta.Repositories.Implementations;

public class ProveedorRepository(IMongoCollection<Proveedor> collection) : IProveedorRepository {
    public async Task<Result<(IReadOnlyList<Proveedor> Proveedores, int Total), DomainErrors>> GetAllAsync(
        string? nombre, int pagina, int tamanoPagina, CancellationToken cancellationToken = default) {
        try {
            var filtro = string.IsNullOrWhiteSpace(nombre)
                ? FilterDefinition<Proveedor>.Empty
                : Builders<Proveedor>.Filter.Regex(p => p.Nombre, new BsonRegularExpression(nombre, "i"));

            var total = (int)await collection.CountDocumentsAsync(filtro, cancellationToken: cancellationToken);

            var proveedores = await collection
                .Find(filtro)
                .SortBy(p => p.Nombre)
                .Skip((pagina - 1) * tamanoPagina)
                .Limit(tamanoPagina)
                .ToListAsync(cancellationToken);

            return Result.Success<(IReadOnlyList<Proveedor>, int), DomainErrors>((proveedores, total));
        }
        catch (Exception) {
            return Result.Failure<(IReadOnlyList<Proveedor>, int), DomainErrors>(ProveedorRepositoryError
                .GetAllError());
        }
    }

    public async Task<Result<Proveedor?, DomainErrors>> GetByIdAsync(ObjectId id,
        CancellationToken cancellationToken = default) {
        try {
            var proveedor = await collection.Find(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);

            return Result.Success<Proveedor?, DomainErrors>(proveedor);
        }
        catch (Exception) {
            return Result.Failure<Proveedor?, DomainErrors>(ProveedorRepositoryError.GetByIdError());
        }
    }

    public async Task<Result<Proveedor, DomainErrors>> AddAsync(Proveedor proveedor,
        CancellationToken cancellationToken = default) {
        try {
            await collection.InsertOneAsync(proveedor, cancellationToken: cancellationToken);

            return Result.Success<Proveedor, DomainErrors>(proveedor);
        }
        catch (Exception) {
            return Result.Failure<Proveedor, DomainErrors>(ProveedorRepositoryError.CreationError());
        }
    }

    public async Task<Result<Proveedor, DomainErrors>> UpdateAsync(Proveedor proveedor,
        CancellationToken cancellationToken = default) {
        try {
            var result = await collection.ReplaceOneAsync(p => p.Id == proveedor.Id, proveedor,
                cancellationToken: cancellationToken);

            return result.IsAcknowledged && result.MatchedCount > 0
                ? Result.Success<Proveedor, DomainErrors>(proveedor)
                : Result.Failure<Proveedor, DomainErrors>(ProveedorRepositoryError.UpdatedError());
        }
        catch (Exception) {
            return Result.Failure<Proveedor, DomainErrors>(ProveedorRepositoryError.UpdatedError());
        }
    }

    public async Task<Result<bool, DomainErrors>> DeleteAsync(Proveedor proveedor,
        CancellationToken cancellationToken = default) {
        try {
            var result = await collection.DeleteOneAsync(p => p.Id == proveedor.Id, cancellationToken);

            return Result.Success<bool, DomainErrors>(result.IsAcknowledged && result.DeletedCount > 0);
        }
        catch (Exception) {
            return Result.Failure<bool, DomainErrors>(ProveedorRepositoryError.DeletedError());
        }
    }

    public async Task<Result<bool, DomainErrors>> ExistsByNameAsync(string nombre, ObjectId? idExcluir = null,
        CancellationToken cancellationToken = default) {
        try {
            var filtro = Builders<Proveedor>.Filter.Regex(p => p.Nombre,
                new BsonRegularExpression($"^{Regex.Escape(nombre)}$", "i"));

            if (idExcluir is not null)
                filtro = Builders<Proveedor>.Filter.And(filtro,
                    Builders<Proveedor>.Filter.Ne(p => p.Id, idExcluir.Value));

            var exists = await collection.CountDocumentsAsync(filtro, cancellationToken: cancellationToken) > 0;

            return Result.Success<bool, DomainErrors>(exists);
        }
        catch (Exception) {
            return Result.Failure<bool, DomainErrors>(ProveedorRepositoryError.ExistsByNameError());
        }
    }
}