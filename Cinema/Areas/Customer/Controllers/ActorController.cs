using Cinema.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(string Name , int page = 1)
        {
            var actors = _context.Actors.Include(m => m.Movies).AsQueryable();

            // filter 
            if(Name != null)
            {
                actors = actors.Where(a => a.Name.Contains(Name));
            }

            // pagination
            ViewBag.Actors = _context.Actors.ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(actors.Count() / 4.0);
            ViewBag.CurrentPage = page;

            actors = actors.Skip((page - 1) * 4).Take(4);
            return View(actors.ToList());
        }
    }
}
