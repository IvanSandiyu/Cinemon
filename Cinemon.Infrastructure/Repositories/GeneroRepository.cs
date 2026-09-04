using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Generos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Repositories
{
    public class GeneroRepository : IGeneroRepository
    {
        private readonly CinemonDbContext _context;
        public GeneroRepository(CinemonDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistAllAsync(IReadOnlyCollection<int> generoIds,CancellationToken cancellationToken)
        {
            var cantidad = await _context.Generos.CountAsync(
                    x => generoIds.Contains(x.Id),cancellationToken);

            return cantidad == generoIds.Count;
        }

        public async Task<IReadOnlyCollection<Genero>> ObtenerTodosAsync(CancellationToken cancellationToken)
        {
            return await _context.Generos
                .AsNoTracking()
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);
        }
    }
}
