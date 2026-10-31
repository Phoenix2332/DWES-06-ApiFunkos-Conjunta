namespace ApiFunkosConjunta.Common.Pagination;

public class PagedResult<T> {
    public IReadOnlyList<T> Datos { get; set; } = [];

    public int Total { get; set; }

    public int Pagina { get; set; }

    public int TamanoPagina { get; set; }

    public int TotalPaginas => (int)Math.Ceiling((double)Total / TamanoPagina);
}