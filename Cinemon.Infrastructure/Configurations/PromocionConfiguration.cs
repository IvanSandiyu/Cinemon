using Cinemon.Domain.Entidades.Promociones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Configurations
{
    public class PromocionConfiguration : IEntityTypeConfiguration<Promocion>
    {
        public void Configure(EntityTypeBuilder<Promocion> builder)
        {
            builder.ToTable("Promociones");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Descripcion)
                .HasMaxLength(500);

            builder.Property(x => x.Tipo)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.CantidadPagadas);
            builder.Property(x => x.CantidadGratis);

            builder.Property(x => x.PorcentajeDescuento)
                .HasPrecision(5,2);

            builder.Property(x => x.FechaDesde)
                .IsRequired();

            builder.Property(x => x.FechaHasta)
                .IsRequired();

            builder.Property(x => x.Activa)
                .IsRequired();

            builder.HasMany(x => x.DiasSemana)
                .WithOne()
                .HasForeignKey(x => x.PromocionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Activa);
        }
    }

    public class PromocionDiaConfiguration : IEntityTypeConfiguration<PromocionDia>
    {
        public void Configure(EntityTypeBuilder<PromocionDia> builder)
        {
            builder.ToTable("PromocionesDias");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Dia)
                .IsRequired()
                .HasConversion<int>();

            builder.HasIndex(x => new
            {
                x.PromocionId,
                x.Dia
            }).IsUnique();
        }
    }
}
