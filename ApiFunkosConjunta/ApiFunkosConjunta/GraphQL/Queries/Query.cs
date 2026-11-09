using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.GraphQL.Types;
using ApiFunkosConjunta.Services.Interfaces;

namespace ApiFunkosConjunta.GraphQL.Queries;

public class Query {
    public string Estado() {
        return "API Funkos GraphQL funcionando";
    }

    public async Task<FunkosPaginados> GetFunkosAsync([Service] IFunkoService funkoService, string? nombre = null,
        int pagina = 1, int tamanoPagina = 10, CancellationToken cancellationToken = default) {
        var result = await funkoService.GetAllAsync(nombre, pagina, tamanoPagina, cancellationToken);

        if (result.IsFailure)
            throw new GraphQLException(result.Error.ToString());

        return new FunkosPaginados(result.Value.Funkos, result.Value.Total, pagina, tamanoPagina);
    }

    [GraphQLName("funko")]
    public async Task<FunkoDto> GetFunkoAsync([Service] IFunkoService funkoService, int id,
        CancellationToken cancellationToken = default) {
        var result = await funkoService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
            throw new GraphQLException(result.Error.ToString());

        return result.Value;
    }

    [GraphQLName("categorias")]
    public async Task<CategoriasPaginadas> GetCategoriasAsync([Service] ICategoriaService categoriaService,
        string? nombre = null, int pagina = 1, int tamanoPagina = 10, CancellationToken cancellationToken = default) {
        var result = await categoriaService.GetAllAsync(nombre, pagina, tamanoPagina, cancellationToken);

        if (result.IsFailure)
            throw new GraphQLException(result.Error.ToString());

        return new CategoriasPaginadas(result.Value.Categorias, result.Value.Total, pagina, tamanoPagina);
    }

    [GraphQLName("categoria")]
    public async Task<CategoriaDto> GetCategoriaAsync([Service] ICategoriaService categoriaService, string id,
        CancellationToken cancellationToken = default) {
        var result = await categoriaService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
            throw new GraphQLException(result.Error.ToString());

        return result.Value;
    }
}