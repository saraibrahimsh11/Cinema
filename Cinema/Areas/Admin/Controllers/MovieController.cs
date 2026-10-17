using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            return View(_context.Movies.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            var movies = _context.Movies.AsQueryable();
            return View(movies.AsEnumerable());
        }
        [HttpPost]
        public IActionResult Create(Movie movie , IFormFile ImageFile)
        {
            if(ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }

                movie.MainImg = fileName;
            }
            _context.Movies.Add(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var movie = _context.Movies.FirstOrDefault(c => c.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        [HttpPost]
        public IActionResult Update(Movie movie , IFormFile ImageFile)
        {
            var movieInDb = _context.Movies.AsNoTracking().FirstOrDefault(b => b.Id == movie.Id);

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }

                movie.MainImg = fileName;

                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", movieInDb.MainImg);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            else
            {
                movie.MainImg = movieInDb.MainImg;
            }
            _context.Movies.Update(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.FirstOrDefault(c => c.Id == id);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", movie.MainImg);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
