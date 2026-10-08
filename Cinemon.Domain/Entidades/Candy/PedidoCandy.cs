using Cinemon.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinemon.Domain.Entidades.Candy
{
    public class PedidoCandy
    {
        public int Id { get; private set; }

        public int UsuarioId { get; private set; }

        public DateTime FechaPedido { get; private set; }

        public decimal Total { get; private set; }

        public ICollection<PedidoCandyItem> Items { get; private set; } = [];

        private PedidoCandy()
        {
        }

        public PedidoCandy(int usuarioId, IEnumerable<PedidoCandyItem> items)
        {
            if (usuarioId <= 0)
                throw new BusinessRuleException("El pedido debe estar asociado a un usuario.");

            var lista = items?.ToList() ?? [];

            if (lista.Count == 0)
                throw new BusinessRuleException("El pedido debe tener al menos un producto.");

            if (lista.Any(x => x.Cantidad < 1))
                throw new BusinessRuleException("La cantidad de cada producto debe ser al menos 1.");

            UsuarioId = usuarioId;
            FechaPedido = DateTime.UtcNow;

            foreach (var item in lista)
                Items.Add(item);

            Total = Items.Sum(x => x.Subtotal);
        }
    }
}
