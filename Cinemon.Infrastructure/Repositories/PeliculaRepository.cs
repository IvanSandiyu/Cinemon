using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Peliculas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Repositories
{
    public class PeliculaRepository : IPeliculaRepository
    {
        private readonly CinemonDbContext _context;

        public PeliculaRepository(CinemonDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Pelicula pelicula,IReadOnlyCollection<int> generoIds,CancellationToken cancellationToken)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            try {
                await _context.Peliculas.AddAsync(
                    pelicula,
                    cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                var relaciones = generoIds.Select(
                    generoId => new PeliculaGenero(
                        pelicula.Id,
                        generoId));

                await _context.Set<PeliculaGenero>()
                    .AddRangeAsync(
                        relaciones,
                        cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);
            } catch {
                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }
        public async Task AddGenerosAsync(Pelicula pelicula,IReadOnlyCollection<int> generoIds,CancellationToken cancellationToken)
        {
            var relaciones = generoIds.Select(
                generoId => new PeliculaGenero(
                    pelicula.Id,
                    generoId));

            await _context.Set<PeliculaGenero>()
                .AddRangeAsync(
                    relaciones,
                    cancellationToken);
        }

        public async Task UpdateAsync(Pelicula pelicula,IReadOnlyCollection<int> generoIds,CancellationToken cancellationToken)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            try {
                pelicula.Generos.Clear();

                _context.Peliculas.Update(pelicula);

                await _context.SaveChangesAsync(
                    cancellationToken);

                var relacionesExistentes = await _context
                    .Set<PeliculaGenero>()
                    .Where(x => x.PeliculaId == pelicula.Id)
                    .ToListAsync(cancellationToken);

                var idsExistentes = relacionesExistentes
                    .Select(x => x.GeneroId)
                    .ToHashSet();

                var idsDeseados = generoIds.ToHashSet();

                var aEliminar = relacionesExistentes
                    .Where(x => !idsDeseados.Contains(x.GeneroId))
                    .ToList();

                var aAgregar = generoIds
                    .Where(generoId => !idsExistentes.Contains(generoId))
                    .Select(generoId => new PeliculaGenero(
                        pelicula.Id,
                        generoId));

                _context.Set<PeliculaGenero>()
                    .RemoveRange(aEliminar);

                await _context.Set<PeliculaGenero>()
                    .AddRangeAsync(
                        aAgregar,
                        cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);
            } catch {
                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<IReadOnlyCollection<Pelicula>> ObtenerTodasAsync(CancellationToken cancellationToken)
        {
            return await _context.Peliculas
                .AsNoTracking()
                .Include(p => p.Generos)
                .ThenInclude(g => g.Genero)
                .ToListAsync(cancellationToken);
        }

        public async Task<Pelicula?> ObtenerPorIdAsync(int id,CancellationToken cancellationToken)
        {
            return await _context.Peliculas
                .AsNoTracking()
                .Include(p => p.Generos)
                .ThenInclude(g => g.Genero)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> CambiarEstadoActivaAsync(int id,bool activa,CancellationToken cancellationToken)
        {
            var pelicula = await _context.Peliculas
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (pelicula is null)
                return false;

            if (activa)
                pelicula.Activar();
            else
                pelicula.Desactivar();

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task UpdateAsync(Pelicula pelicula,CancellationToken cancellationToken)
        {
            _context.Peliculas.Update(pelicula);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
