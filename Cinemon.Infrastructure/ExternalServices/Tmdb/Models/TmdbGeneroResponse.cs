using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.ExternalServices.Tmdb.Models
{
    public sealed record TmdbGeneroResponse(
    [property: JsonPropertyName("id")]int Id,

    [property: JsonPropertyName("name")]string Nombre);
}
