using System;

namespace Каталог_фильмов.Models
{
    public class KinopoiskFilm
    {
        public int FilmId { get; set; }
        public string NameRu { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }
        public string PosterUrl { get; set; }
    }
}
