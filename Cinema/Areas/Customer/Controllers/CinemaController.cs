using Cinema.Data;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(string Name , int page = 1)
        {
            var cinemas = _context.Cinemas.AsQueryable();

            // filter 
            if(Name != null)
            {
                cinemas = cinemas.Where(a => a.Name.Contains(Name));
            }

            // pagination
            ViewBag.Cinemas = _context.Cinemas.ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(cinemas.Count() / 4.0);
            ViewBag.CurrentPage = page;

            cinemas = cinemas.Skip((page - 1) * 4).Take(4);
            return View(cinemas.ToList());
        }
    }
}
