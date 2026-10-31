using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Repositories.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ApiFunkosConjunta.Repositories.Implementations;

public class CategoriaRepository(AppDbContext context) : ICategoriaRepository {
    public async Task<Result<(IReadOnlyList<Categoria> Categorias, int Total), DomainErrors>> GetAllAsync(
        string? nombre, int pagina, int tamanoPagina, CancellationToken cancellationToken = default) {
        try {
            var query = context.Categorias.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(c => c.Nombre.ToLower().Contains(nombre.ToLower()));

            var total = await query.CountAsync(cancellationToken);

            var categorias = await query
                .OrderBy(c => c.Nombre)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync(cancellationToken);

            return Result.Success<(IReadOnlyList<Categoria>, int), DomainErrors>((categorias, total));
        }
        catch (Exception) {
            return Result.Failure<(IReadOnlyList<Categoria>, int), DomainErrors>(RepositoryError.GetAllError());
        }
    }

    public async Task<Result<Categoria?, DomainErrors>> GetByIdAsync(string id,
        CancellationToken cancellationToken = default) {
        try {
            var categoria = await context.Categorias.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            return Result.Success<Categoria?, DomainErrors>(categoria);
        }
        catch (Exception) {
            return Result.Failure<Categoria?, DomainErrors>(RepositoryError.GetByIdError());
        }
    }

    public async Task<Result<Categoria, DomainErrors>> AddAsync(Categoria categoria,
        CancellationToken cancellationToken = default) {
        try {
            await context.Categorias.AddAsync(categoria, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success<Categoria, DomainErrors>(categoria);
        }
        catch (Exception) {
            return Result.Failure<Categoria, DomainErrors>(RepositoryError.CreationError());
        }
    }

    public async Task<Result<Categoria, DomainErrors>> UpdateAsync(Categoria categoria,
        CancellationToken cancellationToken = default) {
        try {
            context.Categorias.Update(categoria);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success<Categoria, DomainErrors>(categoria);
        }
        catch (Exception) {
            return Result.Failure<Categoria, DomainErrors>(RepositoryError.UpdatedError());
        }
    }

    public async Task<Result<bool, DomainErrors>> DeleteAsync(Categoria categoria,
        CancellationToken cancellationToken = default) {
        try {
            context.Categorias.Remove(categoria);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, DomainErrors>(true);
        }
        catch (Exception) {
            return Result.Failure<bool, DomainErrors>(RepositoryError.DeletedError());
        }
    }

    public async Task<Result<bool, DomainErrors>> ExistsByNameAsync(string nombre, string? idExcluir = null,
        CancellationToken cancellationToken = default) {
        try {
            var query = context.Categorias
                .AsNoTracking()
                .Where(c => c.Nombre.ToLower() == nombre.ToLower());

            if (idExcluir is not null) query = query.Where(c => c.Id != idExcluir);

            var exists = await query.AnyAsync(cancellationToken);

            return Result.Success<bool, DomainErrors>(exists);
        }
        catch (Exception) {
            return Result.Failure<bool, DomainErrors>(RepositoryError.ExistsByNameError());
        }
    }
}