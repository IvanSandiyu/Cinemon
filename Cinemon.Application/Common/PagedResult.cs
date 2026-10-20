using System;
using System.Collections.Generic;

namespace Cinemon.Application.Common
{
    public sealed record PagedResult<T>(
        IReadOnlyCollection<T> Items,
        int Pagina,
        int TamanoPagina,
        int Total)
    {
        public int TotalPaginas => TamanoPagina <= 0
            ? 0
            : (int)Math.Ceiling(Total / (double)TamanoPagina);
    }
}
