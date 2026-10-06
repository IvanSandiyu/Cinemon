using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinemon.Domain.Entidades.Promociones
{
    /// <summary>
    /// Promoción configurable como dato: define vigencia, días de la semana
    /// y tipo de descuento (NxM o porcentaje).
    /// </summary>
    public class Promocion
    {
        public int Id { get; private set; }

        public string Nombre { get; private set; }

        public string? Descripcion { get; private set; }

        public TipoPromocion Tipo { get; private set; }

        /// <summary>
        /// NxM: cuántas entradas se cobran. Null para porcentaje.
        /// </summary>
        public int? CantidadPagadas { get; private set; }

        /// <summary>
        /// NxM: cuántas entradas salen gratis. Null para porcentaje.
        /// </summary>
        public int? CantidadGratis { get; private set; }

        /// <summary>
        /// Porcentaje de descuento (0-100). Null para NxM.
        /// </summary>
        public decimal? PorcentajeDescuento { get; private set; }

        public DateTime FechaDesde { get; private set; }

        public DateTime FechaHasta { get; private set; }

        public bool Activa { get; private set; }

        public ICollection<PromocionDia> DiasSemana { get; private set; } = [];

        /// <summary>
        /// Solo para EF Core: los días se cargan por navegación.
        /// </summary>
        private Promocion()
        {
        }

        public Promocion(
            string nombre,
            string? descripcion,
            TipoPromocion tipo,
            int? cantidadPagadas,
            int? cantidadGratis,
            decimal? porcentajeDescuento,
            DateTime fechaDesde,
            DateTime fechaHasta,
            IEnumerable<DayOfWeek> diasSemana)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new BusinessRuleException("La promoción debe tener un nombre.");

            if (diasSemana is null || !diasSemana.Any())
                throw new BusinessRuleException("La promoción debe tener al menos un día de la semana.");

            if (diasSemana.Any(x => !Enum.IsDefined(x)))
                throw new BusinessRuleException("La promoción tiene un día de la semana inválido.");

            if (fechaHasta.Date < fechaDesde.Date)
                throw new BusinessRuleException("La fecha de fin no puede ser anterior a la de inicio.");

            ValidarParametrosSegunTipo(
                tipo,
                cantidadPagadas,
                cantidadGratis,
                porcentajeDescuento);

            Nombre = nombre.Trim();
            Descripcion = descripcion?.Trim();
            Tipo = tipo;
            CantidadPagadas = cantidadPagadas;
            CantidadGratis = cantidadGratis;
            PorcentajeDescuento = porcentajeDescuento;
            FechaDesde = fechaDesde.Date;
            FechaHasta = fechaHasta.Date;
            Activa = true;

            foreach (var dia in diasSemana.Distinct())
                DiasSemana.Add(new PromocionDia(dia));
        }

        private static void ValidarParametrosSegunTipo(
            TipoPromocion tipo,
            int? cantidadPagadas,
            int? cantidadGratis,
            decimal? porcentajeDescuento)
        {
            switch (tipo)
            {
                case TipoPromocion.NxM:
                    if (cantidadPagadas is null or < 1)
                        throw new BusinessRuleException(
                            "Una promoción NxM debe tener al menos una entrada a cobrar.");

                    if (cantidadGratis is null or < 1)
                        throw new BusinessRuleException(
                            "Una promoción NxM debe tener al menos una entrada gratis.");

                    if (porcentajeDescuento is not null)
                        throw new BusinessRuleException(
                            "Una promoción NxM no admite porcentaje de descuento.");

                    break;

                case TipoPromocion.Porcentaje:
                    if (porcentajeDescuento is null or <= 0 or > 100)
                        throw new BusinessRuleException(
                            "El porcentaje de descuento debe estar entre 1 y 100.");

                    if (cantidadPagadas is not null || cantidadGratis is not null)
                        throw new BusinessRuleException(
                            "Una promoción por porcentaje no admite cantidades NxM.");

                    break;

                default:
                    throw new BusinessRuleException("Tipo de promoción desconocido.");
            }
        }

        public void Actualizar(
            string nombre,
            string? descripcion,
            TipoPromocion tipo,
            int? cantidadPagadas,
            int? cantidadGratis,
            decimal? porcentajeDescuento,
            DateTime fechaDesde,
            DateTime fechaHasta,
            IEnumerable<DayOfWeek> diasSemana)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new BusinessRuleException("La promoción debe tener un nombre.");

            if (diasSemana is null || !diasSemana.Any())
                throw new BusinessRuleException("La promoción debe tener al menos un día de la semana.");

            if (diasSemana.Any(x => !Enum.IsDefined(x)))
                throw new BusinessRuleException("La promoción tiene un día de la semana inválido.");

            if (fechaHasta.Date < fechaDesde.Date)
                throw new BusinessRuleException("La fecha de fin no puede ser anterior a la de inicio.");

            ValidarParametrosSegunTipo(
                tipo,
                cantidadPagadas,
                cantidadGratis,
                porcentajeDescuento);

            Nombre = nombre.Trim();
            Descripcion = descripcion?.Trim();
            Tipo = tipo;
            CantidadPagadas = cantidadPagadas;
            CantidadGratis = cantidadGratis;
            PorcentajeDescuento = porcentajeDescuento;
            FechaDesde = fechaDesde.Date;
            FechaHasta = fechaHasta.Date;

            DiasSemana.Clear();

            foreach (var dia in diasSemana.Distinct())
                DiasSemana.Add(new PromocionDia(dia));
        }

        public void Activar() => Activa = true;

        public void Desactivar() => Activa = false;

        /// <summary>
        /// La promoción aplica por vigencia, día de la semana y estado activo.
        /// </summary>
        public bool AplicaPara(DateTime fechaHoraFuncion)
        {
            if (!Activa)
                return false;

            var fecha = fechaHoraFuncion.Date;

            if (fecha < FechaDesde || fecha > FechaHasta)
                return false;

            return DiasSemana.Any(x => x.Dia == fechaHoraFuncion.DayOfWeek);
        }

        /// <summary>
        /// Para NxM indica cuántas entradas se cobran; para porcentaje
        /// devuelve la cantidad completa porque el descuento no depende
        /// del conteo sino del subtotal.
        /// </summary>
        public int CalcularEntradasAPagar(DateTime fechaHoraFuncion,int entradas)
        {
            if (entradas <= 0)
                return 0;

            if (!AplicaPara(fechaHoraFuncion))
                return entradas;

            return Tipo == TipoPromocion.NxM
                ? CalcularEntradasPagadasNxM(entradas)
                : entradas;
        }

        private int CalcularEntradasPagadasNxM(int entradas)
        {
            var pagadas = CantidadPagadas!.Value;
            var gratis = CantidadGratis!.Value;

            // Solo se completan grupos de (pagadas + gratis).
            // Ej: 2x1 con 4 entradas -> 1 grupo completo -> pagan 3.
            var gruposCompletos = entradas / (pagadas + gratis);

            var entradasGratis = gruposCompletos * gratis;

            return entradas - entradasGratis;
        }

        public decimal CalcularTotal(DateTime fechaHoraFuncion,decimal precio,int entradas)
        {
            var subtotal = precio * entradas;

            if (!AplicaPara(fechaHoraFuncion))
                return subtotal;

            return Tipo switch
            {
                TipoPromocion.NxM => precio * CalcularEntradasPagadasNxM(entradas),
                TipoPromocion.Porcentaje => Math.Round(
                    subtotal * (1 - (PorcentajeDescuento!.Value / 100m)),
                    2,
                    MidpointRounding.AwayFromZero),
                _ => subtotal
            };
        }
    }
}
