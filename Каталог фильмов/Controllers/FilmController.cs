using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;
using Каталог_фильмов.DataRepository;
using Каталог_фильмов.Models;

namespace Каталог_фильмов.Controllers
{
    public class FilmController : Controller
    {
        private readonly ILogger _logger;
        private readonly DBRepository _db;

        public FilmController(ILogger<FilmController> logger, DBRepository db)
        {
            _logger = logger;
            _db = db;
        }
        //Список фильмов
        public async Task<IActionResult> Index(int page = 1, string search = null)
        {
            try
            {
                IOrderedQueryable<Film> films = _db.GetFilms(search);
                var model = await PagingList.CreateAsync(films, 10, page);
                return View(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return RedirectToAction("error", "film");
            }
        }
        //Список фильмов с сайта 'Кинопоиск'
        public async Task<IActionResult> FilmsFromKinopoisk(string search)
        {
            try
            {
                KinopoiskFilm[] films = await _db.GetFilmsFromKinopoisk(search);
                return View(films);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return RedirectToAction("error", "film");
            }
        }
        //Список обзоров на фильм с сайта 'Кинопоиск'
        public async Task<IActionResult> Reviews(int id, string title, string description)
        {
            try
            {
                Review[] reviews = await _db.GetReviewsOnTheFilm(id);
                return View(new ReviewViewModel { FilmTitle = title, FilmDescription = description, Reviews = reviews });
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return RedirectToAction("error", "film");
            }
        }
        //Подробности фильма
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                Film film = await _db.GetFilmById(id);
                if (film != null)
                {
                    return View(film);
                }
                else
                {
                    TempData["message2"] = "Фильма не существует";
                    return RedirectToAction("index", "film");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                TempData["message2"] = "Ой! Что - то пошло не так. Попробуйте позже.";
                return RedirectToAction("index", "film");
            }
        }
        //Страница добавления нового фильма
        [Authorize]
        public IActionResult AddFilm()
        {
            return View();
        }
        //Добавление фильма
        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFilm(FilmViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    await _db.AddNewFilm(model);
                    TempData["message"] = "Фильм добавлен";
                    return RedirectToAction("addfilm", "film");
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    TempData["message2"] = "Ой! Что - то пошло не так. Попробуйте позже.";
                    return RedirectToAction("index", "film");
                }
            }
            else
            {
                return View(model);
            }
        }
        //Удаление фильма
        [Authorize]
        public async Task<IActionResult> Delete(int id, string userId)
        {
            try
            {
                string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (userId != UserId)
                {
                    TempData["message2"] = "Вы можете удалять только, добавленные вами, фильмы";
                    return RedirectToAction("index", "film");
                }
                else
                {
                    Film film = await _db.DeleteFilm(id);
                    if (film != null)
                    {
                        TempData["message2"] = "Фильм удалён";
                        return RedirectToAction("index", "film");
                    }
                    else
                    {
                        TempData["message2"] = "Фильма не существует";
                        return RedirectToAction("index", "film");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                TempData["message2"] = "Ой! Что - то пошло не так. Попробуйте позже.";
                return RedirectToAction("index", "film");
            }
        }
        //Страница редактирования фильма
        [Authorize]
        public async Task<IActionResult> EditFilm(int id, string userId)
        {
            try
            {
                string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                Film film = await _db.GetFilmById(id);
                if (film != null)
                {
                    if (UserId == userId)
                    {
                        EditFilmViewModel model = new EditFilmViewModel { Id = film.Id, Title = film.Title, Regisseur = film.Regisseur, YearofManufacture = film.YearofManufacture, Poster = film.Poster, Description = film.Description, UserId = film.UserId, PosterAsString = Convert.ToBase64String(film.Poster) };
                        return View(model);
                    }
                    else
                    {
                        TempData["message2"] = "Вы можете редактировать только, добавленные вами, фильмы";
                        return RedirectToAction("index", "film");
                    }
                }
                else
                {
                    TempData["message2"] = "Фильма не существует";
                    return RedirectToAction("index", "film");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                TempData["message2"] = "Ой! Что - то пошло не так. Попробуйте позже.";
                return RedirectToAction("index", "film");
            }
        }
        //Редактирование фильма
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFilm(EditFilmViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Film film = await _db.EditFilm(model);
                    if (film != null)
                    {
                        TempData["message3"] = "Фильм изменён";
                        return RedirectToAction("editfilm", "film", new { model.Id, model.UserId });
                    }
                    else
                    {
                        TempData["message2"] = "Фильма не существует";
                        return RedirectToAction("index", "film");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    TempData["message2"] = "Ой! Что - то пошло не так. Попробуйте позже.";
                    return RedirectToAction("index", "film");
                }
            }
            else
            {
                model.Poster = Convert.FromBase64String(model.PosterAsString);
                return View(model);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
