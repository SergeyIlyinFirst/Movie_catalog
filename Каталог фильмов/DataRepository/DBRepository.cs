using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public IOrderedQueryable<Film> GetFilms(string search)
        {
            if (search != null)
            {
                search = search.ToLower();
                return db.Films.Include(x => x.User).AsNoTracking().Where(x => x.Description.Contains(search) || x.Regisseur.Contains(search) || x.Title.Contains(search) || x.YearofManufacture.ToString().Contains(search)).Select(x => new Film { Id = x.Id, Title = x.Title, Regisseur = x.Regisseur, YearofManufacture = x.YearofManufacture, UserId = x.UserId, User = x.User }).OrderByDescending(x => x.Id);
            }
            else
            {
                return db.Films.Include(x => x.User).AsNoTracking().Select(x => new Film { Id = x.Id, Title = x.Title, Regisseur = x.Regisseur, YearofManufacture = x.YearofManufacture, UserId = x.UserId, User = x.User }).OrderByDescending(x => x.Id);
            }
        }
        /// <summary>
        /// Найти фильмы на сайте 'Кинопоиск'
        /// </summary>
        /// <returns>KinopoiskFilm[]</returns>
        public async Task<KinopoiskFilm[]> GetFilmsFromKinopoisk (string search)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri($"https://kinopoiskapiunofficial.tech/api/v2.1/films/search-by-keyword?keyword={search}&page=1"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("X-API-KEY", "47b6e527-0f8d-494c-9d12-2b9c43d9db7b");
            HttpResponseMessage response = await client.SendAsync(request);
            HttpContent responseContent = response.Content;
            string json = await responseContent.ReadAsStringAsync();
            var result = JsonConvert.DeserializeAnonymousType(json, new { films = new KinopoiskFilm[]{ } });
            return result.films;
        }
        /// <summary>
        /// Получить рецензии на фильм с сайта 'Кинопоиск'
        /// </summary>
        /// <returns>Review[]</returns>
        public async Task<Review[]> GetReviewsOnTheFilm(int id)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri($"https://kinopoiskapiunofficial.tech/api/v1/reviews?filmId={id}&page=1"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("X-API-KEY", "47b6e527-0f8d-494c-9d12-2b9c43d9db7b");
            HttpResponseMessage response = await client.SendAsync(request);
            HttpContent responseContent = response.Content;
            string json = await responseContent.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json))
            {
                json = "{\"reviews\":[]}";
            }
            var result = JsonConvert.DeserializeAnonymousType(json, new { reviews = new Review[] { } });
            return result.reviews;
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
