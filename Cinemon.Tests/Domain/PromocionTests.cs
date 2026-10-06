using Cinemon.Domain.Entidades.Promociones;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using Cinemon.Domain.Promociones;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cinemon.Tests.Domain
{
    public class PromocionTests
    {
        // Fecha fija y conocida: 2024-01-01 fue un lunes.
        private static readonly DateTime Lunes = new(2024,1,1,20,0,0);
        private static readonly DateTime Martes = Lunes.AddDays(1);
        private static readonly DateTime Miercoles = Lunes.AddDays(2);
        private static readonly DateTime Jueves = Lunes.AddDays(3);

        private static Promocion CrearNxM(
            int pagadas = 2,
            int gratis = 1,
            DateTime? desde = null,
            DateTime? hasta = null,
            bool activa = true,
            params DayOfWeek[] dias)
        {
            var promocion = new Promocion(
                "NxM",
                null,
                TipoPromocion.NxM,
                pagadas,
                gratis,
                null,
                desde ?? new DateTime(2024,1,1),
                hasta ?? new DateTime(2024,12,31),
                dias.Length > 0
                    ? dias
                    : [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday]);

            if (!activa)
                promocion.Desactivar();

            return promocion;
        }

        private static Promocion CrearPorcentaje(
            decimal porcentaje,
            DateTime? desde = null,
            DateTime? hasta = null,
            bool activa = true,
            params DayOfWeek[] dias)
        {
            var promocion = new Promocion(
                "Porcentaje",
                null,
                TipoPromocion.Porcentaje,
                null,
                null,
                porcentaje,
                desde ?? new DateTime(2024,1,1),
                hasta ?? new DateTime(2024,12,31),
                dias.Length > 0 ? dias : Enum.GetValues<DayOfWeek>());

            if (!activa)
                promocion.Desactivar();

            return promocion;
        }

        // --------------------------------------------------------------
        // AplicaPara
        // --------------------------------------------------------------

        [Theory]
        [InlineData(0)] // lunes
        [InlineData(1)] // martes
        [InlineData(2)] // miércoles
        public void Aplica_EnLosDiasConfigurados(int offset)
        {
            var promocion = CrearNxM();

            Assert.True(promocion.AplicaPara(Lunes.AddDays(offset)));
        }

        [Fact]
        public void NoAplica_EnDiasNoConfigurados()
        {
            var promocion = CrearNxM();

            Assert.False(promocion.AplicaPara(Jueves));
        }

        [Fact]
        public void NoAplica_SiEstaInactiva()
        {
            var promocion = CrearNxM(activa: false);

            Assert.False(promocion.AplicaPara(Lunes));
        }

        [Fact]
        public void Aplica_SoloEnLosDiasHabilesDeLaSemana()
        {
            // Mayo 2024: 06 y 08 son lunes y miércoles.
            var promocion = CrearNxM();

            Assert.True(promocion.AplicaPara(new DateTime(2024,5,6,20,0,0)));
            Assert.True(promocion.AplicaPara(new DateTime(2024,5,8,20,0,0)));
            Assert.False(promocion.AplicaPara(new DateTime(2024,5,9,20,0,0)));
        }

        [Fact]
        public void Aplica_InclusiveEnLosLimitesDeVigencia()
        {
            // Ambos límites son lunes.
            var desde = new DateTime(2024,5,6);
            var hasta = new DateTime(2024,5,27);

            var promocion = CrearNxM(desde: desde,hasta: hasta);

            Assert.True(promocion.AplicaPara(new DateTime(2024,5,6,0,0,0)));
            Assert.True(promocion.AplicaPara(new DateTime(2024,5,27,23,59,59)));
            Assert.False(promocion.AplicaPara(new DateTime(2024,6,3,20,0,0)));
            Assert.False(promocion.AplicaPara(new DateTime(2024,4,29,20,0,0)));
        }

        [Fact]
        public void NoAplica_AntesDeLaVigencia()
        {
            var promocion = CrearNxM(
                desde: new DateTime(2024,5,6),
                hasta: new DateTime(2024,5,27));

            Assert.False(promocion.AplicaPara(new DateTime(2024,4,29,20,0,0)));
        }

        // --------------------------------------------------------------
        // Cálculo NxM
        // --------------------------------------------------------------

        // NxM = se pagan N entradas y las M siguientes son gratis.
// Solo se completan grupos de (N + M).
[Theory]
        [InlineData(1,1)] // grupo incompleto
        [InlineData(2,2)] // grupo incompleto
        [InlineData(3,2)] // 1 grupo completo -> 1 gratis
        [InlineData(4,3)]
        [InlineData(5,4)]
        [InlineData(6,4)] // 2 grupos -> 2 gratis
        [InlineData(7,5)]
        public void NxM_SoloDescuentaGruposCompletos(int entradas,int esperado)
        {
            var promocion = CrearNxM(pagadas: 2,gratis: 1);

            Assert.Equal(esperado,promocion.CalcularEntradasAPagar(Lunes,entradas));
        }

        [Fact]
        public void NxM_ConDosEntradasNoGeneraDescuento()
        {
            var promocion = CrearNxM();

            Assert.Equal(2,promocion.CalcularEntradasAPagar(Lunes,2));
        }

        [Fact]
        public void NxM_ConUnaSolaEntradaNoGeneraDescuento()
        {
            var promocion = CrearNxM();

            Assert.Equal(1,promocion.CalcularEntradasAPagar(Lunes,1));
        }

        [Fact]
        public void NxM_SinPromocionCobraTodasLasEntradas()
        {
            var promocion = CrearNxM();

            Assert.Equal(4,promocion.CalcularEntradasAPagar(Jueves,4));
        }

        [Fact]
        public void NxM_TresPorDos()
        {
            // grupo de 5 entradas: se pagan 3
            var promocion = CrearNxM(pagadas: 3,gratis: 2);

            Assert.Equal(4,promocion.CalcularEntradasAPagar(Lunes,4)); // grupo incompleto
            Assert.Equal(3,promocion.CalcularEntradasAPagar(Lunes,5)); // 1 grupo completo
            Assert.Equal(5,promocion.CalcularEntradasAPagar(Lunes,7));
            Assert.Equal(6,promocion.CalcularEntradasAPagar(Lunes,10)); // 2 grupos
        }

        [Fact]
        public void EntradasCero_NoCobraNada()
        {
            var promocion = CrearNxM();

            Assert.Equal(0,promocion.CalcularEntradasAPagar(Lunes,0));
        }

        // --------------------------------------------------------------
        // Cálculo porcentaje
        // --------------------------------------------------------------

        [Fact]
        public void Porcentaje_AplicaElDescuentoSobreElSubtotal()
        {
            var promocion = CrearPorcentaje(20m);

            // 2 entradas x 10000 = 20000; con 20% off -> 16000
            Assert.Equal(16000m,promocion.CalcularTotal(Lunes,10000m,2));
        }

        [Fact]
        public void Porcentaje_NoAplicaEnDiasNoConfigurados()
        {
            var promocion = CrearPorcentaje(20m,dias: [DayOfWeek.Monday]);

            Assert.Equal(20000m,promocion.CalcularTotal(Jueves,10000m,2));
        }

        [Fact]
        public void Porcentaje_RedondeaADosDecimales()
        {
            var promocion = CrearPorcentaje(33m);

            // 3 x 9999 = 29997; 33% = 9899.01
            Assert.Equal(20097.99m,promocion.CalcularTotal(Lunes,9999m,3));
        }

        // --------------------------------------------------------------
        // Validaciones de la entidad
        // --------------------------------------------------------------

        [Fact]
        public void NoSePuedeCrearSinNombre()
        {
            Assert.Throws<BusinessRuleException>(() => new Promocion(
                "  ",
                null,
                TipoPromocion.NxM,
                2,
                1,
                null,
                new DateTime(2024,1,1),
                new DateTime(2024,12,31),
                [DayOfWeek.Monday]));
        }

        [Fact]
        public void NoSePuedeCrearSinDias()
        {
            Assert.Throws<BusinessRuleException>(() => new Promocion(
                "2x1",
                null,
                TipoPromocion.NxM,
                2,
                1,
                null,
                new DateTime(2024,1,1),
                new DateTime(2024,12,31),
                []));
        }

        [Fact]
        public void NoSePuedeCrearConVigenciaInvertida()
        {
            Assert.Throws<BusinessRuleException>(() => new Promocion(
                "2x1",
                null,
                TipoPromocion.NxM,
                2,
                1,
                null,
                new DateTime(2024,12,31),
                new DateTime(2024,1,1),
                [DayOfWeek.Monday]));
        }

        [Theory]
        [InlineData(0,1)]
        [InlineData(2,0)]
        public void NxM_ExigeCantidadesValidas(int pagadas,int gratis)
        {
            Assert.Throws<BusinessRuleException>(() => new Promocion(
                "NxM",
                null,
                TipoPromocion.NxM,
                pagadas,
                gratis,
                null,
                new DateTime(2024,1,1),
                new DateTime(2024,12,31),
                [DayOfWeek.Monday]));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        [InlineData(-5)]
        public void Porcentaje_ExigeRangoValido(decimal porcentaje)
        {
            Assert.Throws<BusinessRuleException>(() => new Promocion(
                "Porcentaje",
                null,
                TipoPromocion.Porcentaje,
                null,
                null,
                porcentaje,
                new DateTime(2024,1,1),
                new DateTime(2024,12,31),
                [DayOfWeek.Monday]));
        }

        [Fact]
        public void NxM_NoAdmitePorcentaje()
        {
            Assert.Throws<BusinessRuleException>(() => new Promocion(
                "NxM",
                null,
                TipoPromocion.NxM,
                2,
                1,
                20m,
                new DateTime(2024,1,1),
                new DateTime(2024,12,31),
                [DayOfWeek.Monday]));
        }

        [Fact]
        public void Porcentaje_NoAdmiteCantidadesNxM()
        {
            Assert.Throws<BusinessRuleException>(() => new Promocion(
                "Porcentaje",
                null,
                TipoPromocion.Porcentaje,
                2,
                1,
                20m,
                new DateTime(2024,1,1),
                new DateTime(2024,12,31),
                [DayOfWeek.Monday]));
        }

        [Fact]
        public void SeActivaYDesactiva()
        {
            var promocion = CrearNxM(activa: false);

            Assert.False(promocion.Activa);

            promocion.Activar();

            Assert.True(promocion.Activa);
            Assert.True(promocion.AplicaPara(Lunes));
        }

        [Fact]
        public void Actualizar_ReemplazaLosDias()
        {
            var promocion = CrearNxM();

            Assert.True(promocion.AplicaPara(Lunes));

            promocion.Actualizar(
                "Nuevo",
                "desc",
                TipoPromocion.NxM,
                2,
                1,
                null,
                new DateTime(2024,1,1),
                new DateTime(2024,12,31),
                [DayOfWeek.Friday]);

            Assert.False(promocion.AplicaPara(Lunes));
            Assert.True(promocion.AplicaPara(new DateTime(2024,1,5,20,0,0)));
            Assert.Equal("Nuevo",promocion.Nombre);
        }
    }
}
