using Cinemon.Application.Abstractions;
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

        public async Task<bool> ExistAllAsync(
            IReadOnlyCollection<int> generoIds,
            CancellationToken cancellationToken)
        {
            var cantidad = await _context.Generos
                .CountAsync(
                    x => generoIds.Contains(x.Id),
                    cancellationToken);

            return cantidad == generoIds.Count;
        }
    }
}
