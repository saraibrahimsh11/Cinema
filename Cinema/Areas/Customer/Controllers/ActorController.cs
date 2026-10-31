using Cinema.Data;
using Cinema.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ActorController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepositry<Actor> _actorRepositry;// = new Repositry<Actor>();

        public ActorController(IRepositry<Actor> actorRepositry)
        {
            _actorRepositry = actorRepositry;
        }

        public async Task<IActionResult> Index(string Name , int page = 1)
        {
            //var actors = _context.Actors.Include(m => m.Movies).AsQueryable();
            var actors = await _actorRepositry.GetListAsync(includes: [m => m.Movies]);

            // filter 
            if (Name != null)
            {
                actors = actors.Where(a => a.Name.Contains(Name));
            }

            // pagination
            //ViewBag.Actors = _context.Actors.ToList();
            ViewBag.Actors = ( await _actorRepositry.GetListAsync()).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(actors.Count() / 4.0);
            ViewBag.CurrentPage = page;

            actors = actors.Skip((page - 1) * 4).Take(4);
            return View(actors.ToList());
        }
    }
}
