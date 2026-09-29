using Cinemon.Domain.Entidades.Precios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Configurations
{
    public class PrecioConfiguration : IEntityTypeConfiguration<Precio>
    {
        public void Configure(EntityTypeBuilder<Precio> builder)
        {
            builder.ToTable("Precios");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Formato)
                .IsRequired();

            builder.Property(x => x.TipoSala)
                .IsRequired();

            builder.Property(x => x.Valor)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.Formato,
                x.TipoSala
            }).IsUnique();
        }
    }
}
