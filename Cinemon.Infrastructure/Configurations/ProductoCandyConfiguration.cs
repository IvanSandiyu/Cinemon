using Cinemon.Domain.Entidades.Candy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinemon.Infrastructure.Configurations
{
    public class ProductoCandyConfiguration : IEntityTypeConfiguration<ProductoCandy>
    {
        public void Configure(EntityTypeBuilder<ProductoCandy> builder)
        {
            builder.ToTable("ProductosCandy");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Descripcion)
                .HasMaxLength(500);

            builder.Property(x => x.Precio)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(x => x.Categoria)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.Activo)
                .IsRequired();

            builder.Property(x => x.FechaCreacion)
                .IsRequired();

            builder.HasMany(x => x.Componentes)
                .WithOne(x => x.Producto)
                .HasForeignKey(x => x.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Activo);

            builder.HasIndex(x => x.Categoria);
        }
    }

    public class ProductoComboItemConfiguration : IEntityTypeConfiguration<ProductoComboItem>
    {
        public void Configure(EntityTypeBuilder<ProductoComboItem> builder)
        {
            builder.ToTable("ProductoComboItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Cantidad)
                .IsRequired();

            builder.HasOne(x => x.Producto)
                .WithMany(x => x.Componentes)
                .HasForeignKey(x => x.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Componente)
                .WithMany()
                .HasForeignKey(x => x.ComponenteProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ProductoId, x.ComponenteProductoId })
                .IsUnique();
        }
    }
}
