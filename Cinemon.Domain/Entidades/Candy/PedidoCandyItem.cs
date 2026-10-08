using Cinemon.Domain.Exceptions;

namespace Cinemon.Domain.Entidades.Candy
{
    public class PedidoCandyItem
    {
        public int Id { get; private set; }

        public int PedidoId { get; private set; }

        public int ProductoId { get; private set; }

        public string NombreProducto { get; private set; } = string.Empty;

        public decimal PrecioUnitario { get; private set; }

        public int Cantidad { get; private set; }

        public decimal Subtotal => PrecioUnitario * Cantidad;

        public PedidoCandy? Pedido { get; private set; }

        private PedidoCandyItem()
        {
        }

        public PedidoCandyItem(
            int productoId,
            string nombreProducto,
            decimal precioUnitario,
            int cantidad)
        {
            if (productoId <= 0)
                throw new BusinessRuleException("El producto del pedido no es válido.");

            if (string.IsNullOrWhiteSpace(nombreProducto))
                throw new BusinessRuleException("El producto del pedido debe tener un nombre.");

            if (precioUnitario <= 0)
                throw new BusinessRuleException("El precio del producto debe ser mayor a cero.");

            if (cantidad < 1)
                throw new BusinessRuleException("La cantidad de cada producto debe ser al menos 1.");

            ProductoId = productoId;
            NombreProducto = nombreProducto.Trim();
            PrecioUnitario = precioUnitario;
            Cantidad = cantidad;
        }
    }
}
