using ApiFunkosConjunta.Dto.Funkos;

namespace ApiFunkosConjunta.GraphQL.Types;

public record FunkosPaginados(IReadOnlyList<FunkoDto> Items, int Total, int Pagina, int TamanoPagina);