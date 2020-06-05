using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Каталог_фильмов.Models
{
    public class EditFilmViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название")]
        [StringLength(50, ErrorMessage = "Длина названия должна быть от 3 до 50 символов", MinimumLength = 3)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Укажите описание")]
        [StringLength(200, ErrorMessage = "Длина описания должна быть от 10 до 200 символов", MinimumLength = 10)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Укажите год создания")]
        [DataType(DataType.Date)]
        public DateTime? YearofManufacture { get; set; }

        [Required(ErrorMessage = "Укажите режиссёра")]
        [StringLength(50, ErrorMessage = "Длина поля 'Режиссёр' должна быть от 3 до 50 символов", MinimumLength = 3)]
        public string Regisseur { get; set; }

        public byte[] Poster { get; set; }

        public string PosterAsString { get; set; }

        public string UserId { get; set; }

        public IFormFile Image { get; set; }

    }
}
