using Cinema.Data;
using Cinema.Models;
using Cinema.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var vm = new HomeVM
            {
                NowShowing = _context.Movies.Include(m => m.Category).Include(m => m.Cinema).Where(m => m.Status == 1)
                    .OrderByDescending(m => m.DateTime).Take(4).ToList(),

                ComingSoon = _context.Movies.Include(m => m.Category).Include(m => m.Cinema).Where(m => m.Status == 2)
                .OrderBy(m => m.DateTime).Take(4).ToList(),

                MoviesCount = _context.Movies.Count(),
                ActorsCount = _context.Actors.Count(),
                CategoriesCount = _context.Categories.Count(),
                CinemasCount = _context.Cinemas.Count()
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
