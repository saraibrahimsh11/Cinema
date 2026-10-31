using Cinema.Data;
using Cinema.Models;
using Cinema.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MovieController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepositry<Movie> _movieRepositry;// = new Repositry<Movie>();

        public MovieController(IRepositry<Movie> movieRepositry)
        {
            _movieRepositry = movieRepositry;
        }

        public async Task<IActionResult> Index(Movie movie , int page = 1 , string Actors = null)
        {
            //var movies = _context.Movies.AsQueryable();
            var movies = await _movieRepositry.GetListAsync(includes: [m => m.Category, m => m.Cinema]);
            //movies = movies.Include(m => m.Category).Include(m => m.Cinema);

            // filter 
            if (movie.Name != null)
            {
                movies = movies.Where(a => a.Name.Contains(movie.Name));
            }

            if (movie.Status != null && movie.Status > 0 && movie.Status <= 3)
            {
                movies = movies.Where(a => a.Status == movie.Status);
            }

            if (movie.Price != null && movie.Price > 0)
            {
                movies = movies.Where(a => a.Price <= movie.Price);
            }

            if(movie.Rate != null)
            {
                movies = movies.Where(a => a.Rate >= movie.Rate);
            }

            if(Actors != null)
            {
                movies = movies.Where(a => a.Actors.Any(a => a.Name.Contains(Actors)));
            }

            // pagination
            //ViewBag.Movies = _context.Movies.ToList();
            ViewBag.Movies = (await _movieRepositry.GetListAsync()).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(movies.Count() / 4.0);
            ViewBag.CurrentPage = page;

            movies = movies.Skip((page - 1) * 4).Take(4);
            return View(movies.ToList());
        }
    }
}
