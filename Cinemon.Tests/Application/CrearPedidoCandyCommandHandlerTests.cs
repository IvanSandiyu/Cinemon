using Cinemon.Application.Candy.Pedidos.Commands.CrearPedidoCandy;
using Cinemon.Application.DTOs.Candy;
using Cinemon.Domain.Entidades.Candy;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using Cinemon.Infrastructure.Repositories;
using Cinemon.Tests.Fakes;
using Cinemon.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cinemon.Tests.Application
{
    public class CrearPedidoCandyCommandHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = new();

        void IDisposable.Dispose() => _db.Dispose();

        private CrearPedidoCandyCommandHandler CrearHandler(int usuarioId) =>
            new(
                new ProductoCandyRepository(_db.Context),
                new PedidoCandyRepository(_db.Context),
                new UsuarioRepository(_db.Context),
                new FakeCurrentUserService(usuarioId));

        private async Task<ProductoCandy> CrearComboAsync(decimal precio = 9200m)
        {
            var pochoclo = await _db.CrearProductoCandyAsync(precio: 6500m);
            var gaseosa = await _db.CrearProductoCandyAsync(
                categoria: CategoriaCandy.Gaseosa,
                precio: 4200m);

            return await _db.CrearProductoCandyAsync(
                nombre: "Combo Clásico",
                precio: precio,
                categoria: CategoriaCandy.Combo,
                componentes:
                [
                    new ProductoComboItem(pochoclo.Id, 1),
                    new ProductoComboItem(gaseosa.Id, 1)
                ]);
        }

        [Fact]
        public async Task CreaElPedidoDelUsuarioAutenticadoYSuTotal()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pochoclo = await _db.CrearProductoCandyAsync(precio: 4500m);
            var gaseosa = await _db.CrearProductoCandyAsync(
                categoria: CategoriaCandy.Gaseosa,
                precio: 3000m);

            var id = await CrearHandler(usuario.Id).Handle(
                new CrearPedidoCandyCommand(
                    [
                        new CrearPedidoCandyItemRequest(pochoclo.Id, 2),
                        new CrearPedidoCandyItemRequest(gaseosa.Id, 1)
                    ]),
                CancellationToken.None);

            Assert.True(id > 0);

            var pedido = await _db.Context.PedidosCandy
                .AsNoTracking()
                .Include(x => x.Items)
                .SingleOrDefaultAsync(x => x.Id == id);

            Assert.NotNull(pedido);
            Assert.Equal(usuario.Id, pedido!.UsuarioId);
            Assert.Equal(12000m, pedido.Total);
            Assert.Equal(2, pedido.Items.Count);
        }

        [Fact]
        public async Task AgrupaLasCantidadesDelMismoProducto()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pochoclo = await _db.CrearProductoCandyAsync(precio: 4500m);

            var id = await CrearHandler(usuario.Id).Handle(
                new CrearPedidoCandyCommand(
                    [
                        new CrearPedidoCandyItemRequest(pochoclo.Id, 2),
                        new CrearPedidoCandyItemRequest(pochoclo.Id, 3)
                    ]),
                CancellationToken.None);

            var pedido = await _db.Context.PedidosCandy
                .AsNoTracking()
                .Include(x => x.Items)
                .SingleAsync(x => x.Id == id);

            var item = Assert.Single(pedido.Items);
            Assert.Equal(5, item.Cantidad);
            Assert.Equal(22500m, pedido.Total);
        }

        [Fact]
        public async Task PermiteComprarUnCombo()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var combo = await CrearComboAsync();

            var id = await CrearHandler(usuario.Id).Handle(
                new CrearPedidoCandyCommand(
                    [new CrearPedidoCandyItemRequest(combo.Id, 1)]),
                CancellationToken.None);

            var pedido = await _db.Context.PedidosCandy
                .AsNoTracking()
                .Include(x => x.Items)
                .SingleAsync(x => x.Id == id);

            var item = Assert.Single(pedido.Items);
            Assert.Equal("Combo Clásico", item.NombreProducto);
            Assert.Equal(9200m, item.PrecioUnitario);
        }

        [Fact]
        public async Task FallaSiElProductoNoExiste()
        {
            var usuario = await _db.CrearUsuarioAsync();

            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler(usuario.Id).Handle(
                    new CrearPedidoCandyCommand(
                        [new CrearPedidoCandyItemRequest(9999, 1)]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiElProductoEstaDesactivado()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pochoclo = await _db.CrearProductoCandyAsync(activo: false);

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler(usuario.Id).Handle(
                    new CrearPedidoCandyCommand(
                        [new CrearPedidoCandyItemRequest(pochoclo.Id, 1)]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiElComboTieneUnComponenteDesactivado()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pochoclo = await _db.CrearProductoCandyAsync(precio: 6500m);
            var gaseosa = await _db.CrearProductoCandyAsync(
                categoria: CategoriaCandy.Gaseosa,
                precio: 4200m);

            var combo = await _db.CrearProductoCandyAsync(
                nombre: "Combo Clásico",
                precio: 9200m,
                categoria: CategoriaCandy.Combo,
                componentes:
                [
                    new ProductoComboItem(pochoclo.Id, 1),
                    new ProductoComboItem(gaseosa.Id, 1)
                ]);

            gaseosa.Desactivar();
            await _db.Context.SaveChangesAsync();

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler(usuario.Id).Handle(
                    new CrearPedidoCandyCommand(
                        [new CrearPedidoCandyItemRequest(combo.Id, 1)]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiElUsuarioNoExiste()
        {
            var pochoclo = await _db.CrearProductoCandyAsync();

            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler(9999).Handle(
                    new CrearPedidoCandyCommand(
                        [new CrearPedidoCandyItemRequest(pochoclo.Id, 1)]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiElUsuarioNoEstaActivo()
        {
            var usuario = await _db.CrearUsuarioAsync(activo: false);
            var pochoclo = await _db.CrearProductoCandyAsync();

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler(usuario.Id).Handle(
                    new CrearPedidoCandyCommand(
                        [new CrearPedidoCandyItemRequest(pochoclo.Id, 1)]),
                    CancellationToken.None));
        }
    }
}
