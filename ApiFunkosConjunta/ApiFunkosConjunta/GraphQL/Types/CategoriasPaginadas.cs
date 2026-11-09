using ApiFunkosConjunta.Dto.Categorias;

namespace ApiFunkosConjunta.GraphQL.Types;

public record CategoriasPaginadas(IReadOnlyList<CategoriaDto> Items, int Total, int Pagina, int TamanoPagina);