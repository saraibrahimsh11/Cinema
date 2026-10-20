using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            return View(_context.Actors.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            var actors = _context.Actors.AsQueryable();
            return View(actors.AsEnumerable());
        }
        [HttpPost]
        public IActionResult Create(Actor actor , IFormFile ImageFile)
        {
            if(ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Actor", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }

                actor.Img = fileName;
            }
            _context.Actors.Add(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var actor = _context.Actors.FirstOrDefault(c => c.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }
        [HttpPost]
        public IActionResult Update(Actor actor , IFormFile ImageFile)
        {
            var actorInDb = _context.Actors.AsNoTracking().FirstOrDefault(b => b.Id == actor.Id);

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Actor", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }

                actor.Img = fileName;

                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Actor", actorInDb.Img);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            else
            {
                actor.Img = actorInDb.Img;
            }
            _context.Actors.Update(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.FirstOrDefault(c => c.Id == id);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Actor", actor.Img);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Actors.Remove(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
