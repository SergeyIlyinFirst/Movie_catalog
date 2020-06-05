using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Каталог_фильмов.Models
{
    public class User : IdentityUser
    {
        public List<Film> Films { get; set; }
        public User()
        {
            Films = new List<Film>();
        }
    }
}
