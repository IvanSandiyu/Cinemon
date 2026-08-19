using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Domain.Entidades.Butacas
{
    public class Butaca
    {
        public int Id { get; set; }
        public int SalaId {  get; set; }
        public string Fila {  get; set; }
        public int Numero { get; set; }
        //public string Codigo => $"{Fila}{Numero}";
    }
}
