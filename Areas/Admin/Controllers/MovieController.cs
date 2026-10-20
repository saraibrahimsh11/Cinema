using Cinema.Data;
using Cinema.Models;
using Cinema.ViewModel;
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
            var movies = _context.Movies.AsQueryable()
                .Include(c => c.Cinema).Include(c => c.Category).Include(a => a.Actors);
            return View(movies.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.AsQueryable();
            var cinemas = _context.Cinemas.AsQueryable();
            var actors = _context.Actors.AsQueryable();

            return View(new MovieVM()
            {
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                Actors = actors.ToList()
            });
        }
        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile ImageFile, List<IFormFile> SubImages , List<int> ActorIds)
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
            // Add Actor to Movie in datebase
            movie.Actors = _context.Actors.Where(a => ActorIds.Contains(a.Id)).ToList();
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
            var movie = _context.Movies.Include(m => m.Actors).FirstOrDefault(c => c.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            var categories = _context.Categories.AsQueryable();
            var cinemas = _context.Cinemas.AsQueryable();
            var actors = _context.Actors.AsQueryable();

            return View(new MovieVM()
            {
                Movie = movie,
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                Actors = actors.ToList(),
                MovieSubImages = _context.MovieSubImages.Where(m => m.MovieId == id).ToList()
            });
        }
        [HttpPost]
        public IActionResult Update(Movie movie, IFormFile ImageFile, List<IFormFile> SubImages, List<int> ActorIds)
        {
            var movieInDb = _context.Movies.Include(a => a.Actors).FirstOrDefault(b => b.Id == movie.Id);

            if (movieInDb == null)
            {
                return NotFound();
            }

            movieInDb.Name = movie.Name;
            movieInDb.Description = movie.Description;
            movieInDb.Price = movie.Price;
            movieInDb.Status = movie.Status;
            movieInDb.DateTime = movie.DateTime;
            movieInDb.Quantity = movie.Quantity;
            movieInDb.Rate = movie.Rate;
            movieInDb.CategoryId = movie.CategoryId;
            movieInDb.CinemaId = movie.CinemaId;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var oldFileName = movieInDb.MainImg;

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }


                if (oldFileName != null)
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", oldFileName);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                movieInDb.MainImg = fileName;

            }

            // Update Actor

            movieInDb.Actors.Clear();
            if (ActorIds != null && ActorIds.Count > 0)
            {
                var selectedActors = _context.Actors.Where(a => ActorIds.Contains(a.Id)).ToList();
                foreach (var actor in selectedActors)
                {
                    movieInDb.Actors.Add(actor);
                }
            }

            //SubImages
            if (SubImages != null && SubImages.Count > 0)
            {
                var oldSubImages = _context.MovieSubImages.Where(p => p.MovieId == movie.Id);

                //delete from wwwroot
                foreach(var item in oldSubImages)
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", item.Image);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }


                // delete from Datebase
                _context.MovieSubImages.RemoveRange(oldSubImages);
                _context.SaveChanges();

                foreach (var image in SubImages)
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
                            MovieId = movie.Id
                        });
                    }
                }
                _context.SaveChanges();
            }

           
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.FirstOrDefault(c => c.Id == id);

            if(movie == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", movie.MainImg);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            var oldSubImages = _context.MovieSubImages.Where(p => p.MovieId == movie.Id);

            //delete from wwwroot
            foreach (var item in oldSubImages)
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", item.Image);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }


            // delete from Datebase
            _context.MovieSubImages.RemoveRange(oldSubImages);
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
