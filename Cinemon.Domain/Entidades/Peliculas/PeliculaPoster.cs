using System;

namespace Cinemon.Domain.Entidades.Peliculas
{
    public class PeliculaPoster
    {
        public int PeliculaId { get; private set; }

        public string Ruta { get; private set; }

        public Pelicula Pelicula { get; private set; } = null!;

        public PeliculaPoster()
        {
            
        }
        public PeliculaPoster(int peliculaId, string ruta)
        {
            PeliculaId = peliculaId;
            Ruta = ruta;
        }
    }
}