using Cinemon.Domain.Entidades.Peliculas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinemon.Infrastructure.Configurations
{
    public class PeliculaConfiguration : IEntityTypeConfiguration<Pelicula>
    {
        public void Configure(EntityTypeBuilder<Pelicula> builder)
        {
            builder.ToTable("Peliculas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Titulo)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Sinopsis)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.Duracion)
                .IsRequired();

            builder.Property(x => x.FechaEstreno)
                .IsRequired();

            builder.Property(x => x.ClasificacionEdad)
                .IsRequired();

            builder.Property(x => x.PosterUrl)
                .HasMaxLength(500);

            builder.Property(x => x.TrailerUrl)
                .HasMaxLength(500);

            builder.Property(x => x.Activa)
                .IsRequired();

            builder.Property(x => x.TmdbId)
                .IsRequired(false);

            builder.Property(x => x.TmdbId).IsRequired(false);

            builder.Property(x => x.TmdbPosterPath)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.TmdbBackdropPath)
                .HasMaxLength(500)
                .IsRequired(false);
        }
    }
}
