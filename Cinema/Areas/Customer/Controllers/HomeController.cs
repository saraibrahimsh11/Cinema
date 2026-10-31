using Cinema.Data;
using Cinema.Models;
using Cinema.Repositories;
using Cinema.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
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
            var vm = new HomeVM
            {
                //NowShowing = _context.Movies.Include(m => m.Category).Include(m => m.Cinema).Where(m => m.Status == 1)
                //    .OrderByDescending(m => m.DateTime).Take(4).ToList(),
                NowShowing = (await _movieRepositry.GetListAsync(m => m.Status == 1, includes: [m => m.Category, m => m.Cinema]))
                        .OrderByDescending(m => m.DateTime).Take(4).ToList(),

                //ComingSoon = _context.Movies.Include(m => m.Category).Include(m => m.Cinema).Where(m => m.Status == 2)
                //.OrderBy(m => m.DateTime).Take(4).ToList(),
                ComingSoon = (await _movieRepositry.GetListAsync(m => m.Status == 2, includes: [m => m.Category, m => m.Cinema]))
                        .OrderBy(m => m.DateTime).Take(4).ToList(),

                //MoviesCount = _context.Movies.Count(),
                MoviesCount =  await _movieRepositry.CountAsync(),
                //ActorsCount = _context.Actors.Count(),
                ActorsCount = await _actorRepositry.CountAsync(),
                //CategoriesCount = _context.Categories.Count(),
                CategoriesCount = await _categoryRepositry.CountAsync(),
                //CinemasCount = _context.Cinemas.Count()
                CinemasCount = await _cinemaRepositry.CountAsync()
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
