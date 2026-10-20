using Cinemon.Application.Peliculas.Queries.ObtenerPeliculas;
using Cinemon.Domain.Entidades.Peliculas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IPeliculaRepository
    {
        Task AddAsync(Pelicula pelicula,IReadOnlyCollection<int> generoIds,CancellationToken cancellationToken);

        Task UpdateAsync(Pelicula pelicula, IReadOnlyCollection<int> generoIds, CancellationToken cancellationToken);
        Task UpdateAsync(Pelicula pelicula, CancellationToken cancellationToken);

        Task<bool> CambiarEstadoActivaAsync(int id,bool activa,CancellationToken cancellationToken);

        Task AddGenerosAsync(Pelicula pelicula,IReadOnlyCollection<int> generoIds,CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Pelicula>> ObtenerTodasAsync(CancellationToken cancellationToken);

        Task<(IReadOnlyCollection<Pelicula> Peliculas, int Total)> ObtenerPaginadasAsync(
            FiltroPeliculasPublicas filtro,
            string? busqueda,
            IReadOnlyCollection<int> ids,
            int pagina,
            int tamanoPagina,
            DateTime hoy,
            CancellationToken cancellationToken);

        Task<Pelicula?> ObtenerPorIdAsync(int id,CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
