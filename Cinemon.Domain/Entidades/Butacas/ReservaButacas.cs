using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Domain.Entidades.Butacas
{
    public class ReservaButacas
    {
        public int Id { get; set; }
        public int ReservaId { get; set; }
        public int ButacaId { get; set; }
    }
}
