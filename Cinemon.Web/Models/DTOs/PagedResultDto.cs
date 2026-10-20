namespace Cinemon.Web.Models.DTOs
{
    public sealed record PagedResultDto<T>(
        IReadOnlyCollection<T> Items,
        int Pagina,
        int TamanoPagina,
        int Total,
        int TotalPaginas)
    {
        public static PagedResultDto<T> Vacio(int pagina, int tamanoPagina) =>
            new([], pagina, tamanoPagina, 0, 0);
    }
}
