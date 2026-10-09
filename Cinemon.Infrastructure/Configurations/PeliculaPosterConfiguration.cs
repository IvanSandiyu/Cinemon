using Cinemon.Domain.Entidades.Peliculas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinemon.Infrastructure.Configurations
{
    public class PeliculaPosterConfiguration : IEntityTypeConfiguration<PeliculaPoster>
    {
        public void Configure(EntityTypeBuilder<PeliculaPoster> builder)
        {
            builder.ToTable("PeliculaPosters");

            builder.HasKey(x => new
            {
                x.PeliculaId,
                x.Ruta
            });

            builder.Property(x => x.Ruta)
                .HasMaxLength(500)
                .IsRequired();

            builder.HasOne(x => x.Pelicula)
                .WithMany(x => x.Posters)
                .HasForeignKey(x => x.PeliculaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}