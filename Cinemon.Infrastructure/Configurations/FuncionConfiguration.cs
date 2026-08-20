using Cinemon.Domain.Entidades.Funcion;
using Cinemon.Domain.Entidades.Peliculas;
using Cinemon.Domain.Entidades.Salas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Configurations
{
    public class FuncionConfiguration : IEntityTypeConfiguration<Funcion>
    {
        public void Configure(EntityTypeBuilder<Funcion> builder)
        {
            builder.ToTable("Funciones");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PeliculaId)
                .IsRequired();

            builder.Property(x => x.SalaId)
                .IsRequired();

            builder.Property(x => x.FechaHoraInicio)
                .IsRequired();

            builder.Property(x => x.Idioma)
                .IsRequired();

            builder.Property(x => x.Formato)
                .IsRequired();

            builder.Property(x => x.Precio)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.EstadoFuncion)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.SalaId,
                x.FechaHoraInicio
            });

            builder.HasOne<Pelicula>()
    .WithMany()
    .HasForeignKey(x => x.PeliculaId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Sala>()
                .WithMany()
                .HasForeignKey(x => x.SalaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
