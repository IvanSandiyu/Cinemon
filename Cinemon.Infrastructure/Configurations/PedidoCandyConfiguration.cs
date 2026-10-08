using Cinemon.Domain.Entidades.Candy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinemon.Infrastructure.Configurations
{
    public class PedidoCandyConfiguration : IEntityTypeConfiguration<PedidoCandy>
    {
        public void Configure(EntityTypeBuilder<PedidoCandy> builder)
        {
            builder.ToTable("PedidosCandy");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UsuarioId)
                .IsRequired();

            builder.Property(x => x.FechaPedido)
                .IsRequired();

            builder.Property(x => x.Total)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.HasMany(x => x.Items)
                .WithOne(x => x.Pedido)
                .HasForeignKey(x => x.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UsuarioId);

            builder.HasIndex(x => x.FechaPedido);
        }
    }

    public class PedidoCandyItemConfiguration : IEntityTypeConfiguration<PedidoCandyItem>
    {
        public void Configure(EntityTypeBuilder<PedidoCandyItem> builder)
        {
            builder.ToTable("PedidosCandyItems");

            builder.HasKey(x => x.Id);

            builder.Ignore(x => x.Subtotal);

            builder.Property(x => x.ProductoId)
                .IsRequired();

            builder.Property(x => x.NombreProducto)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PrecioUnitario)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(x => x.Cantidad)
                .IsRequired();

            builder.HasIndex(x => x.PedidoId);
        }
    }
}
