using Cinemon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Domain.Entidades.Funcion
{
    public class Funcion
    {
        public int Id { get; set; }
        public int PeliculaId { get; set; }
        public int SalaId { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public IdiomaFuncion Idioma { get; set; }
        public Formato Formato { get; set; }
        public decimal Precio { get; set; }
        public EstadoFuncion EstadoFuncion{ get; set; }

        public Funcion(
        int peliculaId,
        int salaId,
        DateTime fechaHoraInicio,
        IdiomaFuncion idioma,
        Formato formato,
        decimal precio)
        {
            PeliculaId = peliculaId;
            SalaId = salaId;
            FechaHoraInicio = fechaHoraInicio;
            Idioma = idioma;
            Formato = formato;
            Precio = precio;
            EstadoFuncion = EstadoFuncion.Programada;
        }

        public void Cancelar()
        {
            EstadoFuncion = EstadoFuncion.Cancelada;
        }

        public void Finalizar()
        {
            EstadoFuncion = EstadoFuncion.Finalizada;
        }
    }
}
