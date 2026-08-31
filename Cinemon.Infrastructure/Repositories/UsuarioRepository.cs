using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Usuarios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly CinemonDbContext _context;

        public UsuarioRepository(CinemonDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }
        public async Task AddAsync(Usuario usuario,CancellationToken cancellationToken)
        {
            await _context.Usuarios.AddAsync(
                usuario,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        public async Task<bool> ExistePorEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .AnyAsync(
                    x => x.Email == email,
                    cancellationToken);
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email,CancellationToken cancellationToken)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Email == email,
                    cancellationToken);
        }
    }
}
