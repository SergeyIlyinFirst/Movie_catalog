using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Каталог_фильмов.Models
{
    public class ReviewViewModel
    {
        public string FilmTitle { get; set; }
        public string FilmDescription { get; set; }
        public Review[] Reviews { get; set; }
    }
}
