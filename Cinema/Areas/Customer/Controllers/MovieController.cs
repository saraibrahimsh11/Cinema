using Cinema.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(string Name , int page = 1)
        {
            var movies = _context.Movies.AsQueryable();
            movies = movies.Include(m => m.Category).Include(m => m.Cinema);

            // filter 
            if(Name != null)
            {
                movies = movies.Where(a => a.Name.Contains(Name));
            }

            // pagination
            ViewBag.Movies = _context.Movies.ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(movies.Count() / 4.0);
            ViewBag.CurrentPage = page;

            movies = movies.Skip((page - 1) * 4).Take(4);
            return View(movies.ToList());
        }
    }
}
