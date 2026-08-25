using Cinemon.Domain.Entidades.Generos;
using Cinemon.Domain.Entidades.Peliculas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Configurations
{
    public class PeliculaGeneroConfiguration:IEntityTypeConfiguration<PeliculaGenero>
    {
        public void Configure(EntityTypeBuilder<PeliculaGenero> builder)
        {
            builder.ToTable("PeliculaGenero");

            builder.HasKey(x => new
            {
                x.PeliculaId,
                x.GeneroId
            });

            builder.HasOne<Pelicula>()
                .WithMany()
                .HasForeignKey(x => x.PeliculaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Genero>()
                .WithMany()
                .HasForeignKey(x => x.GeneroId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
