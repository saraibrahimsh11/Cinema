using Cinema.Data;
using Cinema.Models;
using Cinema.Repositories;
using Cinema.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepositry<Movie> _movieRepositry;// = new Repositry<Movie>();
        private readonly IRepositry<Category> _categoryRepositry;// = new Repositry<Category>();
        private readonly IRepositry<Models.Cinema> _cinemaRepositry;// = new Repositry<Models.Cinema>();
        private readonly IRepositry<Actor> _actorRepositry;// = new Repositry<Actor>();
        private readonly IMovieSubImageRepositry _movieSubImageRepositry;// = new MovieSubImageRepositry();

        public MovieController(IRepositry<Movie> movieRepositry, IRepositry<Category> categoryRepositry, IRepositry<Models.Cinema> cinemaRepositry, IRepositry<Actor> actorRepositry, IMovieSubImageRepositry movieSubImageRepositry)
        {
            _movieRepositry = movieRepositry;
            _categoryRepositry = categoryRepositry;
            _cinemaRepositry = cinemaRepositry;
            _actorRepositry = actorRepositry;
            _movieSubImageRepositry = movieSubImageRepositry;
        }

        public async Task<IActionResult> Index()
        {
            //var movies = _context.Movies.AsQueryable()
            //    .Include(c => c.Cinema).Include(c => c.Category).Include(a => a.Actors);

            var movies = await _movieRepositry.GetListAsync(includes: [c => c.Cinema, c => c.Category, a => a.Actors]);
            return View(movies.ToList());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var categories = _context.Categories.AsQueryable();
            var categories = await _categoryRepositry.GetListAsync();
            //var cinemas = _context.Cinemas.AsQueryable();
            var cinemas = await _cinemaRepositry.GetListAsync();
            //var actors = _context.Actors.AsQueryable();
            var actors = await _actorRepositry.GetListAsync();

            return View(new MovieVM()
            {
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                Actors = actors.ToList()
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile ImageFile, List<IFormFile> SubImages , List<int> ActorIds)
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
            //movie.Actors = _context.Actors.Where(a => ActorIds.Contains(a.Id)).ToList();
            movie.Actors = (await _actorRepositry.GetListAsync(a => ActorIds.Contains(a.Id))).ToList();
            //var savedMovie = _context.Movies.Add(movie);
            var savedMovie = await _movieRepositry.AddAsync(movie);
            //_context.SaveChanges();
            await _movieRepositry.CommitAsync();

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

                        //_context.MovieSubImages.Add(new MovieSubImage()
                        //{
                        //    Image = fileName,
                        //    MovieId = savedMovie.Entity.Id
                        //});

                        await _movieSubImageRepositry.AddAsync(new MovieSubImage()
                        {
                            Image = fileName,
                            MovieId = savedMovie.Entity.Id
                        });
                    }
                }
                //_context.SaveChanges();
                await _movieSubImageRepositry.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var movie = _context.Movies.Include(m => m.Actors).FirstOrDefault(c => c.Id == id);
            var movie = await _movieRepositry.GetOneAsync(c => c.Id == id, includes: [m => m.Actors]);
            if (movie == null)
            {
                return NotFound();
            }
            //var categories = _context.Categories.AsQueryable();
            var categories = await _categoryRepositry.GetListAsync();
            //var cinemas = _context.Cinemas.AsQueryable();
            var cinemas = await _cinemaRepositry.GetListAsync();
            //var actors = _context.Actors.AsQueryable();
            var actors = await _actorRepositry.GetListAsync();

            return View(new MovieVM()
            {
                Movie = movie,
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                Actors = actors.ToList(),
                //MovieSubImages = _context.MovieSubImages.Where(m => m.MovieId == id).ToList()
                MovieSubImages = (await _movieSubImageRepositry.GetListAsync(m => m.MovieId == id)).ToList()
            });
        }
        [HttpPost]
        public async Task<IActionResult> Update(Movie movie, IFormFile ImageFile, List<IFormFile> SubImages, List<int> ActorIds)
        {
            //var movieInDb = _context.Movies.Include(a => a.Actors).FirstOrDefault(b => b.Id == movie.Id);
            var movieInDb = await _movieRepositry.GetOneAsync(b => b.Id == movie.Id, includes:[ a => a.Actors]);

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
                //var selectedActors = _context.Actors.Where(a => ActorIds.Contains(a.Id)).ToList();
                var selectedActors = (await _actorRepositry.GetListAsync(a => ActorIds.Contains(a.Id))).ToList();
                foreach (var actor in selectedActors)
                {
                    movieInDb.Actors.Add(actor);
                }
            }

            //SubImages
            if (SubImages != null && SubImages.Count > 0)
            {
                //var oldSubImages = _context.MovieSubImages.Where(p => p.MovieId == movie.Id);
                var oldSubImages = await _movieSubImageRepositry.GetListAsync(p => p.MovieId == movie.Id);

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
                //_context.MovieSubImages.RemoveRange(oldSubImages);
                _movieSubImageRepositry.RemoveRange(oldSubImages);
                //_context.SaveChanges();
                await _movieSubImageRepositry.CommitAsync();

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

                        //_context.MovieSubImages.Add(new MovieSubImage()
                        //{
                        //    Image = fileName,
                        //    MovieId = movie.Id
                        //});

                        await _movieSubImageRepositry.AddAsync(new MovieSubImage()
                        {
                            Image = fileName,
                            MovieId = movie.Id
                        });
                    }
                }
                //_context.SaveChanges();
                await _movieSubImageRepositry.CommitAsync();
            }


            //_context.SaveChanges();
            await _movieRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var movie = _context.Movies.FirstOrDefault(c => c.Id == id);
            var movie = await _movieRepositry.GetOneAsync(c => c.Id == id);

            if(movie == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", movie.MainImg);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            //var oldSubImages = _context.MovieSubImages.Where(p => p.MovieId == movie.Id);
            var oldSubImages = await _movieSubImageRepositry.GetListAsync(p => p.MovieId == movie.Id);

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
            //_context.MovieSubImages.RemoveRange(oldSubImages);
            _movieSubImageRepositry.RemoveRange(oldSubImages);
            //_context.Movies.Remove(movie);
            _movieRepositry.Remove(movie);
            //_context.SaveChanges();
            await _movieRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
