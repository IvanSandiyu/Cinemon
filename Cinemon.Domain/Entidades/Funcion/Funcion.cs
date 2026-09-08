using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Domain.Entidades.Funcion
{
    public class Funcion
    {
        public int Id { get; private set; }
        public int PeliculaId { get; private set; }
        public int SalaId { get; private set; }
        public DateTime FechaHoraInicio { get; private set; }
        public IdiomaFuncion Idioma { get; private set; }
        public Formato Formato { get; private set; }
        public decimal Precio { get; private set; }
        public EstadoFuncion EstadoFuncion{ get; private set; }

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
                throw new BusinessRuleException("Solo se pueden cancelar funciones programadas.");

            EstadoFuncion = EstadoFuncion.Cancelada;
        }

        public void Finalizar()
        {
            if (EstadoFuncion != EstadoFuncion.Programada)
                throw new BusinessRuleException("Solo se pueden finalizar funciones programadas.");

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
