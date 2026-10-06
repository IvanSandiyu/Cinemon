using Cinemon.Domain.Entidades.Promociones;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Promociones;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cinemon.Tests.Domain
{
    public class MotorPromocionesTests
    {
        private static readonly DateTime Lunes = new(2024,1,1,20,0,0);

        private static Promocion NxM(
            int pagadas = 2,
            int gratis = 1,
            params DayOfWeek[] dias)
        {
            return new Promocion(
                $"{pagadas}x{gratis}",
                null,
                TipoPromocion.NxM,
                pagadas,
                gratis,
                null,
                new DateTime(2024,1,1),
                new DateTime(2024,12,31),
                dias.Length > 0
                    ? dias
                    : [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday]);
        }

        private static Promocion Porcentaje(
            decimal porcentaje,
            params DayOfWeek[] dias)
        {
            return new Promocion(
                $"{porcentaje}%",
                null,
                TipoPromocion.Porcentaje,
                null,
                null,
                porcentaje,
                new DateTime(2024,1,1),
                new DateTime(2024,12,31),
                dias.Length > 0 ? dias : Enum.GetValues<DayOfWeek>());
        }

        [Fact]
        public void SinPromociones_CobraElSubtotalCompleto()
        {
            var resultado = MotorPromociones.Calcular([],Lunes,10000m,2);

            Assert.Null(resultado.Promocion);
            Assert.Equal(2,resultado.Entradas);
            Assert.Equal(2,resultado.EntradasAPagar);
            Assert.Equal(20000m,resultado.Subtotal);
            Assert.Equal(20000m,resultado.Total);
            Assert.Equal(0m,resultado.Ahorro);
        }

        [Fact]
        public void AplicaLaPromocionQueCorresponde()
        {
            var resultado = MotorPromociones.Calcular([NxM()],Lunes,10000m,3);

            Assert.NotNull(resultado.Promocion);
            Assert.Equal(2,resultado.EntradasAPagar);
            Assert.Equal(30000m,resultado.Subtotal);
            Assert.Equal(20000m,resultado.Total);
            Assert.Equal(10000m,resultado.Ahorro);
        }

        [Fact]
        public void NxM_NoDescuentaGruposIncompletos()
        {
            var resultado = MotorPromociones.Calcular([NxM()],Lunes,10000m,2);

            Assert.Equal(20000m,resultado.Total);
            Assert.Equal(0m,resultado.Ahorro);
        }

        [Fact]
        public void IgnoraPromocionesQueNoCorrespondenAlDia()
        {
            var resultado = MotorPromociones.Calcular(
                [NxM(dias: [DayOfWeek.Friday])],
                Lunes,
                10000m,
                2);

            Assert.Null(resultado.Promocion);
            Assert.Equal(20000m,resultado.Total);
        }

        [Fact]
        public void EntreVarias_GanaElDescuentoMasAlto()
        {
            // 2x1 sobre 4 entradas paga 2 -> 20000
            // 3x2 sobre 4 entradas paga 2 -> 20000
            // 50% sobre 4 entradas -> 20000
            // 60% sobre 4 entradas -> 16000  <-- gana esta
            var resultado = MotorPromociones.Calcular(
                [
                    NxM(2,1),
                    NxM(3,2),
                    Porcentaje(50m),
                    Porcentaje(60m)
                ],
                Lunes,
                10000m,
                4);

            Assert.Equal(16000m,resultado.Total);
            Assert.Equal("60%",resultado.Promocion?.Nombre);
        }

        [Fact]
        public void EntreVarias_ComparaConVariasCombinaciones()
        {
            // 6 entradas a 10000 (subtotal 60000)
            // 2x1 -> 2 grupos -> pagan 4 -> 40000
            // 3x2 -> 1 grupo de 5 -> pagan 4 -> 40000
            // 40% -> 36000  <-- gana esta
            var resultado = MotorPromociones.Calcular(
                [NxM(2,1),NxM(3,2),Porcentaje(25m),Porcentaje(40m)],
                Lunes,
                10000m,
                6);

            Assert.Equal(36000m,resultado.Total);
            Assert.Equal("40%",resultado.Promocion?.Nombre);
        }

        [Fact]
        public void NoAcumulaDescuentos()
        {
            // Si se acumularan, 2x1 + 20% darían 16000 en vez de 20000.
            var resultado = MotorPromociones.Calcular(
                [NxM(2,1),Porcentaje(20m)],
                Lunes,
                10000m,
                4);

            Assert.Equal(30000m,resultado.Total);
            Assert.Equal("2x1",resultado.Promocion?.Nombre);
        }

        [Fact]
        public void IgnoraPromocionesInactivas()
        {
            var inactiva = NxM();

            inactiva.Desactivar();

            var resultado = MotorPromociones.Calcular([inactiva],Lunes,10000m,2);

            Assert.Null(resultado.Promocion);
            Assert.Equal(20000m,resultado.Total);
        }

        [Fact]
        public void EntradasCero_NoAplicaPromociones()
        {
            var resultado = MotorPromociones.Calcular([NxM()],Lunes,10000m,0);

            Assert.Null(resultado.Promocion);
            Assert.Equal(0,resultado.EntradasAPagar);
            Assert.Equal(0m,resultado.Total);
        }
    }
}
