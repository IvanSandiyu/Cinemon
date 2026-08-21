using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Enums;
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
        public EstadoReserva EstadoReserva { get; set; }
        public decimal Total { get; set; }

        public Reserva(
       int usuarioId,
       int realizadaPorId,
       int funcionId,
       decimal total)
        {
            UsuarioId = usuarioId;
            RealizadaPorId = realizadaPorId;
            FuncionId = funcionId;
            Total = total;
            FechaReserva = DateTime.UtcNow;
            EstadoReserva = EstadoReserva.Confirmada;
        }

        public void Cancelar()
        {
            EstadoReserva = EstadoReserva.Cancelada;
        }
    }
}
