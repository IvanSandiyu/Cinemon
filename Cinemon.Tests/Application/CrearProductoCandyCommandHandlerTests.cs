using Cinemon.Application.Candy.Productos.Commands.CrearProductoCandy;
using Cinemon.Application.DTOs.Candy;
using Cinemon.Domain.Entidades.Candy;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using Cinemon.Infrastructure.Repositories;
using Cinemon.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cinemon.Tests.Application
{
    public class CrearProductoCandyCommandHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = new();

        void IDisposable.Dispose() => _db.Dispose();

        private CrearProductoCandyCommandHandler CrearHandler() =>
            new(new ProductoCandyRepository(_db.Context));

        [Fact]
        public async Task CreaElProductoYDevuelveElIdGenerado()
        {
            var id = await CrearHandler().Handle(
                new CrearProductoCandyCommand(
                    "Pochoclo Grande",
                    "80 g.",
                    6500m,
                    CategoriaCandy.Pochoclo,
                    []),
                CancellationToken.None);

            Assert.True(id > 0);

            var producto = await _db.Context.ProductosCandy
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

            Assert.NotNull(producto);
            Assert.Equal("Pochoclo Grande", producto!.Nombre);
            Assert.Equal(6500m, producto.Precio);
            Assert.True(producto.Activo);
        }

        [Fact]
        public async Task CreaElComboConSusComponentes()
        {
            var pochoclo = await _db.CrearProductoCandyAsync(precio: 6500m);
            var gaseosa = await _db.CrearProductoCandyAsync(
                categoria: CategoriaCandy.Gaseosa,
                precio: 4200m);

            var id = await CrearHandler().Handle(
                new CrearProductoCandyCommand(
                    "Combo Clásico",
                    "Pochoclo grande con gaseosa mediana.",
                    9200m,
                    CategoriaCandy.Combo,
                    [
                        new ComponenteComboRequest(pochoclo.Id, 1),
                        new ComponenteComboRequest(gaseosa.Id, 1)
                    ]),
                CancellationToken.None);

            var combo = await _db.Context.ProductosCandy
                .AsNoTracking()
                .Include(x => x.Componentes)
                .SingleOrDefaultAsync(x => x.Id == id);

            Assert.NotNull(combo);
            Assert.Equal(CategoriaCandy.Combo, combo!.Categoria);
            Assert.Equal(2, combo.Componentes.Count);
            Assert.Contains(combo.Componentes, x => x.ComponenteProductoId == pochoclo.Id);
            Assert.Contains(combo.Componentes, x => x.ComponenteProductoId == gaseosa.Id);
        }

        [Fact]
        public async Task FallaSiElComboCuestaMasQueLaSumaDeSusComponentes()
        {
            var pochoclo = await _db.CrearProductoCandyAsync(precio: 6500m);

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler().Handle(
                    new CrearProductoCandyCommand(
                        "Combo Raro",
                        null,
                        20000m,
                        CategoriaCandy.Combo,
                        [new ComponenteComboRequest(pochoclo.Id, 1)]),
                    CancellationToken.None));

            Assert.Single(await _db.Context.ProductosCandy.ToListAsync());
        }

        [Fact]
        public async Task FallaSiElComboNoTieneComponentes()
        {
            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler().Handle(
                    new CrearProductoCandyCommand(
                        "Combo Vacío",
                        null,
                        9000m,
                        CategoriaCandy.Combo,
                        []),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiElProductoSimpleTieneComponentes()
        {
            var pochoclo = await _db.CrearProductoCandyAsync();

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler().Handle(
                    new CrearProductoCandyCommand(
                        "Pochoclo",
                        null,
                        4500m,
                        CategoriaCandy.Pochoclo,
                        [new ComponenteComboRequest(pochoclo.Id, 1)]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiUnComponenteDelComboNoExiste()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler().Handle(
                    new CrearProductoCandyCommand(
                        "Combo",
                        null,
                        9000m,
                        CategoriaCandy.Combo,
                        [new ComponenteComboRequest(9999, 1)]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiElComboSeArmaConOtroCombo()
        {
            var pochoclo = await _db.CrearProductoCandyAsync(precio: 6500m);

            var combo = await _db.CrearProductoCandyAsync(
                nombre: "Combo Clásico",
                precio: 6500m,
                categoria: CategoriaCandy.Combo,
                componentes: [new ProductoComboItem(pochoclo.Id, 1)]);

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler().Handle(
                    new CrearProductoCandyCommand(
                        "Combo de Combos",
                        null,
                        6500m,
                        CategoriaCandy.Combo,
                        [new ComponenteComboRequest(combo.Id, 1)]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task AgrupaLasCantidadesRepetidasDelMismoComponente()
        {
            var pochoclo = await _db.CrearProductoCandyAsync(precio: 4500m);

            var id = await CrearHandler().Handle(
                new CrearProductoCandyCommand(
                    "Combo Familiar",
                    null,
                    17500m,
                    CategoriaCandy.Combo,
                    [
                        new ComponenteComboRequest(pochoclo.Id, 2),
                        new ComponenteComboRequest(pochoclo.Id, 2)
                    ]),
                CancellationToken.None);

            var combo = await _db.Context.ProductosCandy
                .AsNoTracking()
                .Include(x => x.Componentes)
                .SingleAsync(x => x.Id == id);

            var componente = Assert.Single(combo.Componentes);
            Assert.Equal(4, componente.Cantidad);
        }
    }
}
