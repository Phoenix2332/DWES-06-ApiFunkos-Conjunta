using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Repositories.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ApiFunkosConjunta.Repositories.Implementations;

public class FunkoRepository(AppDbContext context) : IFunkoRepository {
    public async Task<Result<(IReadOnlyList<Funko> Funkos, int Total), DomainErrors>> GetAllAsync(string? nombre,
        int pagina, int tamanoPagina, CancellationToken cancellationToken = default) {
        try {
            var query = context.Funkos.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(f => f.Nombre.ToLower().Contains(nombre.ToLower()));

            var total = await query.CountAsync(cancellationToken);

            var funkos = await query
                .OrderBy(f => f.Nombre)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync(cancellationToken);

            return Result.Success<(IReadOnlyList<Funko>, int), DomainErrors>((funkos, total));
        }
        catch (Exception) {
            return Result.Failure<(IReadOnlyList<Funko>, int), DomainErrors>(RepositoryError.GetAllError());
        }
    }

    public async Task<Result<Funko?, DomainErrors>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default) {
        try {
            var funko = await context.Funkos
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

            return Result.Success<Funko?, DomainErrors>(funko);
        }
        catch (Exception) {
            return Result.Failure<Funko?, DomainErrors>(RepositoryError.GetByIdError());
        }
    }

    public async Task<Result<Funko, DomainErrors>> AddAsync(Funko funko,
        CancellationToken cancellationToken = default) {
        try {
            await context.Funkos.AddAsync(funko, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success<Funko, DomainErrors>(funko);
        }
        catch (Exception) {
            return Result.Failure<Funko, DomainErrors>(RepositoryError.CreationError());
        }
    }

    public async Task<Result<Funko, DomainErrors>> UpdateAsync(Funko funko,
        CancellationToken cancellationToken = default) {
        try {
            context.Funkos.Update(funko);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success<Funko, DomainErrors>(funko);
        }
        catch (Exception) {
            return Result.Failure<Funko, DomainErrors>(RepositoryError.UpdatedError());
        }
    }

    public async Task<Result<bool, DomainErrors>> DeleteAsync(Funko funko,
        CancellationToken cancellationToken = default) {
        try {
            context.Funkos.Remove(funko);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, DomainErrors>(true);
        }
        catch (Exception) {
            return Result.Failure<bool, DomainErrors>(RepositoryError.DeletedError());
        }
    }

    public async Task<Result<bool, DomainErrors>> ExistsByCategoriaIdAsync(string categoriaId,
        CancellationToken cancellationToken = default) {
        try {
            var existe = await context.Funkos
                .AnyAsync(f => f.CategoriaId == categoriaId, cancellationToken);

            return Result.Success<bool, DomainErrors>(existe);
        }
        catch (Exception) {
            return Result.Failure<bool, DomainErrors>(RepositoryError.GetAllError());
        }
    }

    public async Task<Result<bool, DomainErrors>> ExistsByProveedorIdAsync(string proveedorId,
        CancellationToken cancellationToken = default) {
        try {
            var existe = await context.Funkos
                .AnyAsync(f => f.ProveedorId == proveedorId, cancellationToken);

            return Result.Success<bool, DomainErrors>(existe);
        }
        catch (Exception) {
            return Result.Failure<bool, DomainErrors>(RepositoryError.GetAllError());
        }
    }
}