using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Каталог_фильмов.Data;
using Каталог_фильмов.Models;

namespace Каталог_фильмов.DataRepository
{
    public class DBRepository
    {
        private ApplicationDbContext db;
        public DBRepository(ApplicationDbContext context)
        {
            db = context;
        }
        /// <summary>
        /// Получить список фильмов
        /// </summary>
        /// <returns>IOrderedQueryable<Film></returns>
        public IOrderedQueryable<Film> GetFilms()
        {
            return db.Films.Include(x => x.User).AsNoTracking().Select(x => new Film { Id = x.Id, Title = x.Title, Regisseur = x.Regisseur, YearofManufacture = x.YearofManufacture, UserId = x.UserId, User = x.User }).OrderByDescending(x => x.Id);
        }
        /// <summary>
        /// Получить фильм по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Film</returns>
        public async Task<Film> GetFilmById(int id)
        {
            return await db.Films.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }
        /// <summary>
        /// Добавить новый фильм
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task AddNewFilm(FilmViewModel model)
        {
            Film film = new Film { Title = model.Title, Description = model.Description, Regisseur = model.Regisseur, YearofManufacture = model.YearofManufacture.Value, UserId = model.UserId };
            byte[] imageData = null;
            using (var binaryReader = new BinaryReader(model.Poster.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)model.Poster.Length);
            }
            film.Poster = imageData;
            db.Films.Add(film);
            await db.SaveChangesAsync();
        }
        /// <summary>
        /// Сохранить изменённый фильм
        /// </summary>
        /// <param></param>
        /// <returns>Film</returns>
        public async Task<Film> EditFilm(EditFilmViewModel model)
        {
            Film film = await db.Films.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (film != null)
            {
                film.Title = model.Title;
                film.Description = model.Description;
                film.Regisseur = model.Regisseur;
                film.YearofManufacture = model.YearofManufacture.Value;
                film.UserId = model.UserId;
                if (model.Image != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(model.Image.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)model.Image.Length);
                    }
                    film.Poster = imageData;
                }
                await db.SaveChangesAsync();
            }
            return film;
        }
        /// <summary>
        /// Удалить фильм
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Film</returns>
        public async Task<Film> DeleteFilm(int id)
        {
            Film film = db.Films.FirstOrDefault(x => x.Id == id);
            if (film != null)
            {
                db.Remove(film);
                await db.SaveChangesAsync();
            }
            return film;
        }
    }
}
