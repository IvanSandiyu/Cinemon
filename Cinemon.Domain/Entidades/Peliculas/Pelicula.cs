using Cinemon.Domain.Entidades.Generos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Domain.Entidades.Peliculas
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Sipnosis { get; set; }
        public int Duracion {  get; set; }
        public DateTime FechaEstreno { get; set; }
        public ICollection<Genero> Generos { get; set; }
        public string ClasificacionEdad { get; set; }
        public string PosterUrl { get; set; }
        public string TrailerUrl { get; set; }
        public bool Activa { get; set; }
    }
}
