using Cinemon.Domain.Entidades.Butacas;
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
    public class ButacaConfiguration : IEntityTypeConfiguration<Butaca>
    {
        public void Configure(EntityTypeBuilder<Butaca> builder)
        {
            builder.ToTable("Butacas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SalaId)
                .IsRequired();

            builder.Property(x => x.Fila)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(x => x.Numero)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.SalaId,
                x.Fila,
                x.Numero
            })
            .IsUnique();

            builder.HasOne<Sala>().WithMany().HasForeignKey(x => x.SalaId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
