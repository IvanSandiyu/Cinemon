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
        public int Id { get; private set; }
        public int UsuarioId { get; private set; }
        public int RealizadaPorId { get; private set; }
        public int FuncionId { get; private set; }
        public DateTime FechaReserva { get; private set; }
        public EstadoReserva EstadoReserva { get; private set; }
        public decimal Total { get; private set; }

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
