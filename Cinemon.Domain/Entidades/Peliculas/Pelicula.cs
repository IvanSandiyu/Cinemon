using Cinemon.Domain.Entidades.Generos;
using Cinemon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Domain.Entidades.Peliculas
{
    public class Pelicula
    {
        public int Id { get; private set; }
        public string Titulo { get; private set; }
        public string Sinopsis { get; private set; }
        public int Duracion {  get; private set; }
        public DateTime FechaEstreno { get; private set; }
        public ClasificacionEdad ClasificacionEdad { get; private set; }
        public string PosterUrl { get; private set; }
        public string TrailerUrl { get; private set; }
        public bool Activa { get; private set; }
        public ICollection<PeliculaGenero> Generos { get; private set; } = [];

        //Creamos la pelicula y automaticamente Activo es false ya que no nos interesa que se pueda ver todavia
        //Creamos aca y no en application pq es mas sencillo y automaticamente esta en "stand by"
        public Pelicula(
        string titulo,
        string sinopsis,
        int duracion,
        DateTime fechaEstreno,
        ClasificacionEdad clasificacionEdad,
        string posterUrl,
        string trailerUrl)
        {
            Titulo = titulo;
            Sinopsis = sinopsis;
            Duracion = duracion;
            FechaEstreno = fechaEstreno;
            ClasificacionEdad = clasificacionEdad;
            PosterUrl = posterUrl;
            TrailerUrl = trailerUrl;
            Activa = false;
        }

        public void Activar()
        {
            Activa = true;
        }

        public void Desactivar()
        {
            Activa = false;
        }

        public void Actualizar(
            string titulo,
            string sinopsis,
            int duracion,
            DateTime fechaEstreno,
            ClasificacionEdad clasificacionEdad,
            string posterUrl,
            string trailerUrl)
        {
            Titulo = titulo;
            Sinopsis = sinopsis;
            Duracion = duracion;
            FechaEstreno = fechaEstreno;
            ClasificacionEdad = clasificacionEdad;
            PosterUrl = posterUrl;
            TrailerUrl = trailerUrl;
        }
    }
}
