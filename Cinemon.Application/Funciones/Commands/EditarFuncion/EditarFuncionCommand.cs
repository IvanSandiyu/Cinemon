using Cinemon.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Commands.EditarFuncion
{
    public sealed record EditarFuncionCommand(
     int Id,
     int PeliculaId,
     int SalaId,
     DateTime FechaHoraInicio,
     IdiomaFuncion Idioma,
     Formato Formato,
     decimal Precio) : IRequest;
}
