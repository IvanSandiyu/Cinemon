using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Interfaces
{
    public interface ITmdbImageUrlBuilder
    {
        string? ObtenerPosterUrl(string? posterPath);

        string? ObtenerBackdropUrl(string? backdropPath);
    }
}
