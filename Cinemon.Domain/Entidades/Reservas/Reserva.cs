using Cinemon.Domain.Entidades.Butacas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Domain.Entidades.Reservas
{
    public class Reserva
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int RealizadaPorId { get; set; }
        public int FuncionId { get; set; }
        public DateTime FechaReserva { get; set; }
        public bool Estado { get; set; }
        public float Total { get; set; }
        //public ReservaButacas ReservasButacas { get; set; }
    }
}
