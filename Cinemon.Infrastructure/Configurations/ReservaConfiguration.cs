using Cinemon.Domain.Entidades.Funcion;
using Cinemon.Domain.Entidades.Reservas;
using Cinemon.Domain.Entidades.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Configurations
{
    public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
    {
        public void Configure(EntityTypeBuilder<Reserva> builder)
        {
            builder.ToTable("Reservas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UsuarioId)
                .IsRequired();

            builder.Property(x => x.RealizadaPorId)
                .IsRequired();

            builder.Property(x => x.FuncionId)
                .IsRequired();

            builder.Property(x => x.FechaReserva)
                .IsRequired();

            builder.Property(x => x.EstadoReserva)
                .IsRequired();

            builder.Property(x => x.Total)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.RealizadaPorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Funcion>()
                .WithMany()
                .HasForeignKey(x => x.FuncionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.Id,
                x.FuncionId
            }).IsUnique();
        }
    }
}
