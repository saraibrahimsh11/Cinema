using Cinema.Data;
using Cinema.Models;
using Cinema.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var movies = _context.Movies.AsQueryable();
            var vm = new DashboardVM
            {
                MoviesCount = movies.Count(),
                CategoriesCount = _context.Categories.Count(),
                CinemasCount = _context.Cinemas.Count(),
                ActorsCount = _context.Actors.Count(),

                NowShowing = movies.Count(m => m.Status == 1),
                ComingSoon = movies.Count(m => m.Status == 2),
                Ended = movies.Count(m => m.Status == 2),

                AveragePrice = movies.Average(m => m.Price),
                AverageRate = movies.Average(m => m.Rate),
                TotalQuantity = movies.Sum(m => m.Quantity),

                TopRated = movies.Include(m => m.Category).Include(m => m.Cinema)
                    .OrderByDescending(m => m.Rate).Take(5).ToList(),

                Latest = movies.Include(m => m.Category).Include(m => m.Cinema)
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
