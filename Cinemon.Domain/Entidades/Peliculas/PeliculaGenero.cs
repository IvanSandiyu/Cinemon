using Cinemon.Domain.Entidades.Generos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Domain.Entidades.Peliculas
{
    public class PeliculaGenero
    {
        public int PeliculaId { get; private set; }

        public int GeneroId { get; private set; }

        public Pelicula Pelicula { get; private set; } = null!;

        public Genero Genero { get; private set; } = null!;

        public PeliculaGenero()
        {
            
        }
        public PeliculaGenero(int peliculaId, int generoId)
        {
            PeliculaId = peliculaId;
            GeneroId = generoId;
        }
    }
}
