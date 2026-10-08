using Cinemon.Application.Abstractions;
using Cinemon.Application.DTOs.Candy;
using Cinemon.Application.Interfaces;
using Cinemon.Domain.Entidades.Candy;
using Cinemon.Domain.Exceptions;
using MediatR;
using System.Linq;

namespace Cinemon.Application.Candy.Pedidos.Commands.CrearPedidoCandy
{
    public sealed class CrearPedidoCandyCommandHandler
        : IRequestHandler<CrearPedidoCandyCommand, int>
    {
        private readonly IProductoCandyRepository _productoCandyRepository;
        private readonly IPedidoCandyRepository _pedidoCandyRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICurrentUserService _currentUserService;

        public CrearPedidoCandyCommandHandler(
            IProductoCandyRepository productoCandyRepository,
            IPedidoCandyRepository pedidoCandyRepository,
            IUsuarioRepository usuarioRepository,
            ICurrentUserService currentUserService)
        {
            _productoCandyRepository = productoCandyRepository;
            _pedidoCandyRepository = pedidoCandyRepository;
            _usuarioRepository = usuarioRepository;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(
            CrearPedidoCandyCommand request,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(
                _currentUserService.UserId,
                cancellationToken);

            if (usuario is null)
                throw new NotFoundException("Usuario", _currentUserService.UserId);

            if (!usuario.Activo)
                throw new BusinessRuleException("El usuario no está activo.");

            var cantidades = request.Items
                .GroupBy(x => x.ProductoId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Cantidad));

            var productos = await _productoCandyRepository.ObtenerPorIdsAsync(
                cantidades.Keys.ToList(),
                cancellationToken);

            if (productos.Count != cantidades.Count)
                throw new NotFoundException("Uno o más productos del pedido no existen.");

            var items = new List<PedidoCandyItem>();

            foreach (var producto in productos)
            {
                ValidarDisponibilidad(producto);

                items.Add(new PedidoCandyItem(
                    producto.Id,
                    producto.Nombre,
                    producto.Precio,
                    cantidades[producto.Id]));
            }

            var pedido = new PedidoCandy(usuario.Id, items);

            await _pedidoCandyRepository.AgregarAsync(pedido, cancellationToken);

            return pedido.Id;
        }

        private static void ValidarDisponibilidad(ProductoCandy producto)
        {
            if (!producto.Activo)
                throw new BusinessRuleException(
                    $"El producto \"{producto.Nombre}\" no está disponible.");

            if (producto.Categoria != Domain.Enums.CategoriaCandy.Combo)
                return;

            var agotado = producto.Componentes
                .FirstOrDefault(x => x.Componente is null || !x.Componente.Activo);

            if (agotado is not null)
                throw new BusinessRuleException(
                    $"El combo \"{producto.Nombre}\" no está disponible en este momento.");
        }
    }
}
