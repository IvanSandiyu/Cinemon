using Cinemon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Domain.Entidades.Salas
{
    public class Sala
    {
        public int Id {  get; set; }
        public int Numero { get; set; }
        public TipoSala TipoSala { get; set; }
        
    }

}
