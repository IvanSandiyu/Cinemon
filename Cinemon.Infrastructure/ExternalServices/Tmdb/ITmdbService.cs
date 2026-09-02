using Cinemon.Application.DTOs.Tmdb;
using Cinemon.Infrastructure.ExternalServices.Tmdb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.ExternalServices.Tmdb
{
    public interface ITmdbService
    {
        Task<IReadOnlyCollection<TmdbMovieDto>> BuscarPeliculasAsync(string query,CancellationToken cancellationToken);
    }
}
