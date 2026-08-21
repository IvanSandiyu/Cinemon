using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Entidades.Reservas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Configurations
{
    public class ReservaButacaConfiguration: IEntityTypeConfiguration<ReservaButaca>
    {
        public void Configure(EntityTypeBuilder<ReservaButaca> builder)
        {
            builder.ToTable("ReservasButacas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReservaId)
                .IsRequired();

            builder.Property(x => x.FuncionId)
                .IsRequired();

            builder.Property(x => x.ButacaId)
                .IsRequired();

            builder.HasOne<Reserva>()
                .WithMany()
                .HasForeignKey(x => new
                {
                    x.ReservaId,
                    x.FuncionId
                })
                .HasPrincipalKey(x => new
                {
                    x.Id,
                    x.FuncionId
                })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Butaca>()
                .WithMany()
                .HasForeignKey(x => x.ButacaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.FuncionId,
                x.ButacaId
            }).IsUnique();

            builder.Property(x => x.FuncionId).IsRequired();
        }
    }
}
