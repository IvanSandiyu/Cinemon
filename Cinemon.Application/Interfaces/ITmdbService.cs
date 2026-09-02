using Cinemon.Application.DTOs.Tmdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Interfaces
{
    public interface ITmdbService
    {
        Task<IReadOnlyCollection<TmdbMovieDto>> BuscarPeliculasAsync(
            string query,
            CancellationToken cancellationToken);
    }
}
