using System;

namespace Каталог_фильмов.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime YearofManufacture { get; set; }
        public string Regisseur { get; set; }
        public byte[] Poster { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
