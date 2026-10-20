using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            return View(_context.Cinemas.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            var cinemas = _context.Cinemas.AsQueryable();
            return View(cinemas.AsEnumerable());
        }
        [HttpPost]
        public IActionResult Create(Models.Cinema cinema , IFormFile ImageFile)
        {
            if(ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Cinema", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }

                cinema.Img = fileName;
            }
            _context.Cinemas.Add(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(c => c.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }
            return View(cinema);
        }
        [HttpPost]
        public IActionResult Update(Models.Cinema cinema , IFormFile ImageFile)
        {
            var cinemaInDb = _context.Cinemas.AsNoTracking().FirstOrDefault(b => b.Id == cinema.Id);

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Cinema", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }

                cinema.Img = fileName;

                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Cinema", cinemaInDb.Img);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            else
            {
                cinema.Img = cinemaInDb.Img;
            }
            _context.Cinemas.Update(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(c => c.Id == id);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Cinema", cinema.Img);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
