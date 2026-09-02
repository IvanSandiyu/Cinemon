using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.ExternalServices.Tmdb
{
    public sealed class TmdbOptions
    {
        public const string SectionName = "Tmdb";

        public string BaseUrl { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;
    }
}
