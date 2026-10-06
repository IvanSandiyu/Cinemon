namespace Cinemon.Domain.Entidades.Promociones
{
    public class PromocionDia
    {
        public int Id { get; set; }

        public int PromocionId { get; set; }

        public DayOfWeek Dia { get; set; }

        public PromocionDia()
        {
        }

        public PromocionDia(DayOfWeek dia)
        {
            Dia = dia;
        }
    }
}
