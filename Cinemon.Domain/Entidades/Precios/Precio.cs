using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Domain.Entidades.Precios
{
    /// <summary>
    /// Precio oficial de una entrada para una combinación de formato y tipo de sala.
    /// </summary>
    public class Precio
    {
        public int Id { get; private set; }
        public Formato Formato { get; private set; }
        public TipoSala TipoSala { get; private set; }
        public decimal Valor { get; private set; }

        public Precio(Formato formato,TipoSala tipoSala,decimal valor)
        {
            if (valor <= 0) {
                throw new BusinessRuleException(
                    "El precio debe ser mayor a 0.");
            }

            Formato = formato;
            TipoSala = tipoSala;
            Valor = valor;
        }

        public void ActualizarValor(decimal valor)
        {
            if (valor <= 0) {
                throw new BusinessRuleException(
                    "El precio debe ser mayor a 0.");
            }

            Valor = valor;
        }
    }
}
