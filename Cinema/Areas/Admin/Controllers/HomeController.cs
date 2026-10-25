using Cinema.Data;
using Cinema.Models;
using Cinema.Repositories;
using Cinema.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepositry<Movie> _movieRepositry;// = new Repositry<Movie>();
        private readonly IRepositry<Category> _categoryRepositry;// = new Repositry<Category>();
        private readonly IRepositry<Models.Cinema> _cinemaRepositry;// = new Repositry<Models.Cinema>();
        private readonly IRepositry<Actor> _actorRepositry;// = new Repositry<Actor>();

        public HomeController(IRepositry<Movie> movieRepositry, IRepositry<Category> categoryRepositry, IRepositry<Models.Cinema> cinemaRepositry, IRepositry<Actor> actorRepositry)
        {
            _movieRepositry = movieRepositry;
            _categoryRepositry = categoryRepositry;
            _cinemaRepositry = cinemaRepositry;
            _actorRepositry = actorRepositry;
        }

        public async Task<IActionResult> Index()
        {
            //var movies = _context.Movies.AsQueryable();
            var movies = await _movieRepositry.GetListAsync();
            var vm = new DashboardVM
            {
                MoviesCount = movies.Count(),
                //CategoriesCount = _context.Categories.Count(),
                CategoriesCount = await _categoryRepositry.CountAsync(),
                //CinemasCount = _context.Cinemas.Count(),
                CinemasCount = await _cinemaRepositry.CountAsync(),
                //ActorsCount = _context.Actors.Count(),
                ActorsCount = await _actorRepositry.CountAsync(),

                NowShowing = movies.Count(m => m.Status == 1),
                ComingSoon = movies.Count(m => m.Status == 2),
                Ended = movies.Count(m => m.Status == 2),

                AveragePrice = movies.Average(m => m.Price),
                AverageRate = movies.Average(m => m.Rate),
                TotalQuantity = movies.Sum(m => m.Quantity),

                //TopRated = movies.Include(m => m.Category).Include(m => m.Cinema)
                //    .OrderByDescending(m => m.Rate).Take(5).ToList(),
                TopRated = (await _movieRepositry.GetListAsync(includes: [m => m.Category , m => m.Cinema]))
                    .OrderByDescending(m => m.Rate).Take(5).ToList(),

                //Latest = movies.Include(m => m.Category).Include(m => m.Cinema)
                //    .OrderByDescending(m => m.Id).Take(5).ToList(),
                Latest = (await _movieRepositry.GetListAsync(includes: [m => m.Category , m => m.Cinema]))
                       .OrderByDescending(m => m.Id).Take(5).ToList(),
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
