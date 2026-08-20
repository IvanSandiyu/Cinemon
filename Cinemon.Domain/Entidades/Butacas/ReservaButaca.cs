using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Domain.Entidades.Butacas
{
    public class ReservaButaca
    {
        public int Id { get; set; }
        public int ReservaId { get; set; }
        public int FuncionId { get; private set; }
        public int ButacaId { get; set; }

        public ReservaButaca(
         int reservaId,
         int funcionId,
         int butacaId)
        {
            ReservaId = reservaId;
            FuncionId = funcionId;
            ButacaId = butacaId;
        }
    }
}
