using Cinemon.Domain.Entidades.Funcion;
using Cinemon.Domain.Entidades.Peliculas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IFuncionRepository
    {
        Task<bool> ExisteSuperposicionAsync(int salaId,DateTime fechaHoraInicio,DateTime fechaHoraFin,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Funcion>>ObtenerFuncionesAsync(CancellationToken cancellationToken);

        Task<Funcion?> ObtenerPorIdAsync(int id,CancellationToken cancellationToken);

        Task<bool> ExisteSuperposicionAsync(int salaId,DateTime fechaHoraInicio,DateTime fechaHoraFin,
            int? funcionId,
            CancellationToken cancellationToken);

        Task AddAsync(Funcion funcion,CancellationToken cancellationToken);

        Task<Pelicula?> ObtenerPeliculaAsync(int peliculaId,CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);

        Task UpdateAsync(Funcion funcion, CancellationToken cancellationToken);
    }
}
