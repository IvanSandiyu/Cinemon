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
            if (EstadoFuncion != EstadoFuncion.Programada)
                throw new InvalidOperationException(
                    "Solo se pueden cancelar funciones programadas.");

            EstadoFuncion = EstadoFuncion.Cancelada;
        }

        public void Finalizar()
        {
            if (EstadoFuncion != EstadoFuncion.Programada)
                throw new InvalidOperationException(
                    "Solo se pueden finalizar funciones programadas.");

            EstadoFuncion = EstadoFuncion.Finalizada;
        }

        public void Editar(int peliculaId,int salaId,DateTime fechaHoraInicio,IdiomaFuncion idioma,
            Formato formato,
            decimal precio)
        {
            PeliculaId = peliculaId;
            SalaId = salaId;
            FechaHoraInicio = fechaHoraInicio;
            Idioma = idioma;
            Formato = formato;
            Precio = precio;
        }
    }
}
