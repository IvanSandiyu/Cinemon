using Cinemon.Application.Abstractions;
using Cinemon.Application.DTOs.Candy;
using Cinemon.Domain.Entidades.Candy;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cinemon.Application.Candy
{
    internal static class ComponentesComboHelper
    {
        public static async Task<(List<ProductoComboItem> Items, decimal Suma)> ResolverAsync(
            IProductoCandyRepository productoCandyRepository,
            CategoriaCandy categoria,
            IReadOnlyCollection<ComponenteComboRequest>? componentes,
            CancellationToken cancellationToken)
        {
            var pedidos = (componentes ?? [])
                .GroupBy(x => x.ProductoId)
                .Select(g => new ComponenteComboRequest(g.Key, g.Sum(x => x.Cantidad)))
                .ToList();

            if (categoria != CategoriaCandy.Combo)
            {
                if (pedidos.Count > 0)
                    throw new BusinessRuleException("Solo los combos pueden tener componentes.");

                return ([], 0m);
            }

            if (pedidos.Count == 0)
                throw new BusinessRuleException("Un combo debe tener al menos un producto.");

            var productos = await productoCandyRepository.ObtenerPorIdsAsync(
                pedidos.Select(x => x.ProductoId).ToList(),
                cancellationToken);

            if (productos.Count != pedidos.Count)
                throw new NotFoundException("Uno o más productos del combo no existen.");

            if (productos.Any(x => x.Categoria == CategoriaCandy.Combo))
                throw new BusinessRuleException("Un combo no puede estar formado por otro combo.");

            var porId = productos.ToDictionary(x => x.Id);

            var items = pedidos
                .Select(x => new ProductoComboItem(x.ProductoId, x.Cantidad))
                .ToList();

            var suma = pedidos.Sum(x => porId[x.ProductoId].Precio * x.Cantidad);

            return (items, suma);
        }
    }
}
