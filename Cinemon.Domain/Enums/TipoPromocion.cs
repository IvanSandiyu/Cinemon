namespace Cinemon.Domain.Enums
{
    public enum TipoPromocion
    {
        /// <summary>
        /// NxM: se cobran N entradas y las M restantes son gratis.
        /// Ej: 2x1 (2 se pagan, 1 gratis), 3x2.
        /// </summary>
        NxM = 1,

        /// <summary>
        /// Porcentaje de descuento sobre el subtotal.
        /// Ej: 20% off.
        /// </summary>
        Porcentaje = 2
    }
}
