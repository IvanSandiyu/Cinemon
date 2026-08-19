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
        public TipoSala TipoSala { get; set; }
        public Formato Formato { get; set; }
        public float Precio { get; set; }
        public EstadoFuncion EstadoFuncion{ get; set; }
    }
}
