using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Common
{
    public static class TmdbImageUrlHelper
    {
        private const string BaseUrl ="https://image.tmdb.org/t/p/";

        public static string? BuildPosterUrl(string? posterPath)
        {
            if (string.IsNullOrWhiteSpace(posterPath))
                return null;

            return $"{BaseUrl}w500{posterPath}";
        }

        public static string? BuildBackdropUrl(string? backdropPath)
        {
            if (string.IsNullOrWhiteSpace(backdropPath))
                return null;

            return $"{BaseUrl}original{backdropPath}";
        }
    }
}
