using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Peliculas.Commands.VincularTmdb
{
    public sealed record VincularTmdbCommand(int PeliculaId,int TmdbId) : IRequest;
}
