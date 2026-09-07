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

        public async Task<IReadOnlyCollection<Genero>> ObtenerPorNombresAsync(IReadOnlyCollection<string> nombres, CancellationToken cancellationToken)
        {
            return await _context.Generos
                .Where(x => nombres.Contains(x.Nombre))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Genero>> ObtenerOCrearPorNombresAsync(IReadOnlyCollection<string> nombres, CancellationToken cancellationToken)
        {
            var normalizados = nombres
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(Normalizar)
                .ToHashSet();

            if (normalizados.Count == 0)
                return [];

            var existentes = await _context.Generos.ToListAsync(cancellationToken);

            var porNombreNormalizado = existentes
                .ToDictionary(g => Normalizar(g.Nombre), g => g);

            var resultado = new List<Genero>(normalizados.Count);

            foreach (var nombreNormalizado in normalizados)
            {
                if (porNombreNormalizado.TryGetValue(nombreNormalizado, out var genero))
                {
                    resultado.Add(genero);
                    continue;
                }

                var nombreOriginal = nombres.First(n => Normalizar(n) == nombreNormalizado);

                var nuevo = new Genero { Nombre = nombreOriginal.Trim() };
                _context.Generos.Add(nuevo);
                porNombreNormalizado[nombreNormalizado] = nuevo;
                resultado.Add(nuevo);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return resultado;
        }

        private static string Normalizar(string nombre)
        {
            return nombre
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .Aggregate(new System.Text.StringBuilder(), (sb, c) => sb.Append(char.ToLowerInvariant(c)))
                .ToString();
        }
    }
}
