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
            movies = movies.Include(m => m.Category).Include(m => m.Cinema);
            return View(movies.AsEnumerable());
        }
        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile ImageFile, List<IFormFile> SubImages)
        {
            // Add ImageFile to wwwroot and Database
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }

                movie.MainImg = fileName;
            }
            var savedMovie = _context.Movies.Add(movie);
            _context.SaveChanges();

            if(SubImages != null && SubImages.Count > 0)
            {
                foreach(var image in SubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }

                        _context.MovieSubImages.Add(new MovieSubImage()
                        {
                            Image = fileName,
                            MovieId = savedMovie.Entity.Id
                        });
                    }
                }
                _context.SaveChanges();
            }
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
        public IActionResult Update(Movie movie, IFormFile ImageFile)
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
