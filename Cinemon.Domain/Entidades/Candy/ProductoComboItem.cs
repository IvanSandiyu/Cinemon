using Cinemon.Domain.Exceptions;

namespace Cinemon.Domain.Entidades.Candy
{
    public class ProductoComboItem
    {
        public int Id { get; private set; }

        public int ProductoId { get; private set; }

        public int ComponenteProductoId { get; private set; }

        public int Cantidad { get; private set; }

        public ProductoCandy? Producto { get; private set; }

        public ProductoCandy? Componente { get; private set; }

        private ProductoComboItem()
        {
        }

        public ProductoComboItem(int componenteProductoId, int cantidad)
        {
            if (componenteProductoId <= 0)
                throw new BusinessRuleException("El producto que compone el combo no es válido.");

            if (cantidad < 1)
                throw new BusinessRuleException("La cantidad de cada componente debe ser al menos 1.");

            ComponenteProductoId = componenteProductoId;
            Cantidad = cantidad;
        }
    }
}
