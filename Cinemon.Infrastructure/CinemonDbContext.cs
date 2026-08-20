using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Entidades.Funcion;
using Cinemon.Domain.Entidades.Generos;
using Cinemon.Domain.Entidades.Peliculas;
using Cinemon.Domain.Entidades.Reservas;
using Cinemon.Domain.Entidades.Salas;
using Cinemon.Domain.Entidades.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Cinemon.Infrastructure
{
    public class CinemonDbContext : DbContext
    {
        public CinemonDbContext(DbContextOptions<CinemonDbContext> options)
        : base(options)
        {
        }

        public DbSet<Pelicula> Peliculas => Set<Pelicula>();

        public DbSet<Genero> Generos => Set<Genero>();

        public DbSet<Sala> Salas => Set<Sala>();

        public DbSet<Butaca> Butacas => Set<Butaca>();

        public DbSet<Funcion> Funciones => Set<Funcion>();

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        public DbSet<Reserva> Reservas => Set<Reserva>();

        public DbSet<ReservaButaca> ReservasButacas => Set<ReservaButaca>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(CinemonDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
