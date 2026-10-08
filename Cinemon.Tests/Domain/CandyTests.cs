using Cinemon.Domain.Entidades.Candy;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using System;
using System.Linq;
using Xunit;

namespace Cinemon.Tests.Domain
{
    public class CandyTests
    {
        [Fact]
        public void ElProductoNaceActivoYConLaCategoriaElegida()
        {
            var producto = new ProductoCandy(
                "Pochoclo Grande",
                "80 g.",
                6500m,
                CategoriaCandy.Pochoclo);

            Assert.True(producto.Activo);
            Assert.Equal(CategoriaCandy.Pochoclo, producto.Categoria);
            Assert.Equal(6500m, producto.Precio);
            Assert.Empty(producto.Componentes);
        }

        [Fact]
        public void ElProductoNoPuedeLlegarSinNombre()
        {
            Assert.Throws<BusinessRuleException>(() =>
                new ProductoCandy("   ", null, 1000m, CategoriaCandy.Extra));
        }

        [Fact]
        public void ElProductoNoPuedeCostarCero()
        {
            Assert.Throws<BusinessRuleException>(() =>
                new ProductoCandy("Agua mineral", null, 0m, CategoriaCandy.Extra));
        }

        [Fact]
        public void ElNombreSeGuardaSinEspaciosDeMas()
        {
            var producto = new ProductoCandy(
                "  Gaseosa  ",
                null,
                3000m,
                CategoriaCandy.Gaseosa);

            Assert.Equal("Gaseosa", producto.Nombre);
        }

        [Fact]
        public void SoloElComboPuedeTenerComponentes()
        {
            Assert.Throws<BusinessRuleException>(() =>
                new ProductoCandy(
                    "Pochoclo",
                    null,
                    4500m,
                    CategoriaCandy.Pochoclo,
                    [new ProductoComboItem(1, 1)]));
        }

        [Fact]
        public void ElComboDebeTenerAlMenosUnComponente()
        {
            Assert.Throws<BusinessRuleException>(() =>
                new ProductoCandy("Combo", null, 9000m, CategoriaCandy.Combo, []));
        }

        [Fact]
        public void ElComboAgrupaLosComponentesRepetidos()
        {
            var combo = new ProductoCandy(
                "Combo",
                null,
                9000m,
                CategoriaCandy.Combo,
                [
                    new ProductoComboItem(7, 1),
                    new ProductoComboItem(7, 2),
                    new ProductoComboItem(9, 1)
                ]);

            Assert.Equal(2, combo.Componentes.Count);
            Assert.Equal(3, combo.Componentes.Single(x => x.ComponenteProductoId == 7).Cantidad);
            Assert.Equal(1, combo.Componentes.Single(x => x.ComponenteProductoId == 9).Cantidad);
        }

        [Fact]
        public void ElComboNoPuedeCostarMasQueLaSumaDeSusComponentes()
        {
            var combo = new ProductoCandy(
                "Combo",
                null,
                12000m,
                CategoriaCandy.Combo,
                [new ProductoComboItem(7, 1)]);

            Assert.Throws<BusinessRuleException>(() => combo.ValidarPrecioCombo(10000m));
        }

        [Fact]
        public void ElComboPuedeCostarMenosQueLaSumaDeSusComponentes()
        {
            var combo = new ProductoCandy(
                "Combo",
                null,
                9200m,
                CategoriaCandy.Combo,
                [new ProductoComboItem(7, 1)]);

            combo.ValidarPrecioCombo(10700m);
        }

        [Fact]
        public void LaReglaDePrecioDelComboNoAfectaALosProductosSimples()
        {
            var producto = new ProductoCandy(
                "Pochoclo",
                null,
                99999m,
                CategoriaCandy.Pochoclo);

            producto.ValidarPrecioCombo(1m);
        }

        [Fact]
        public void ElProductoSePuedeDesactivarYReactivar()
        {
            var producto = new ProductoCandy(
                "Pochoclo",
                null,
                4500m,
                CategoriaCandy.Pochoclo);

            producto.Desactivar();
            Assert.False(producto.Activo);

            producto.Activar();
            Assert.True(producto.Activo);
        }

        [Fact]
        public void AlActualizarSeReemplazanLosComponentesDelCombo()
        {
            var combo = new ProductoCandy(
                "Combo",
                null,
                9000m,
                CategoriaCandy.Combo,
                [new ProductoComboItem(7, 1)]);

            combo.Actualizar(
                "Combo Pareja",
                null,
                18500m,
                CategoriaCandy.Combo,
                [
                    new ProductoComboItem(7, 2),
                    new ProductoComboItem(9, 2)
                ]);

            Assert.Equal("Combo Pareja", combo.Nombre);
            Assert.Equal(2, combo.Componentes.Count);
            Assert.Equal(2, combo.Componentes.Single(x => x.ComponenteProductoId == 7).Cantidad);
        }

        [Fact]
        public void AlActualizarUnProductoSimpleSeLeQuitanLosComponentes()
        {
            var combo = new ProductoCandy(
                "Combo",
                null,
                9000m,
                CategoriaCandy.Combo,
                [new ProductoComboItem(7, 1)]);

            combo.Actualizar("Pochoclo", null, 4500m, CategoriaCandy.Pochoclo);

            Assert.Empty(combo.Componentes);
        }

        [Fact]
        public void ElPedidoNoPuedeQuedarSinProductos()
        {
            Assert.Throws<BusinessRuleException>(() => new PedidoCandy(1, []));
        }

        [Fact]
        public void ElPedidoNoPuedeAsociarseAUnUsuarioInvalido()
        {
            Assert.Throws<BusinessRuleException>(() =>
                new PedidoCandy(0, [new PedidoCandyItem(1, "Pochoclo", 4500m, 1)]));
        }

        [Fact]
        public void ElPedidoCalculaElTotalASusItems()
        {
            var pedido = new PedidoCandy(
                5,
                [
                    new PedidoCandyItem(1, "Pochoclo", 4500m, 2),
                    new PedidoCandyItem(2, "Gaseosa", 3000m, 1)
                ]);

            Assert.Equal(12000m, pedido.Total);
            Assert.Equal(9000m, pedido.Items.First().Subtotal);
            Assert.Equal(3000m, pedido.Items.Last().Subtotal);
        }

        [Fact]
        public void ElItemDelPedidoConservaElPrecioDeLaVenta()
        {
            var item = new PedidoCandyItem(3, "Combo Clásico", 9200m, 2);

            Assert.Equal(18400m, item.Subtotal);
        }

        [Fact]
        public void ElItemDelPedidoNoAdmitePreciosInvalidos()
        {
            Assert.Throws<BusinessRuleException>(() =>
                new PedidoCandyItem(1, "Pochoclo", 0m, 1));
        }

        [Fact]
        public void ElItemDelPedidoNoAdmiteCantidadesMenoresAUno()
        {
            Assert.Throws<BusinessRuleException>(() =>
                new PedidoCandyItem(1, "Pochoclo", 1000m, 0));
        }
    }
}
