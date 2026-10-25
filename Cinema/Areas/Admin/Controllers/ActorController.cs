using Cinema.Data;
using Cinema.Models;
using Cinema.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepositry<Actor> _actorRepositry;// = new Repositry<Actor>();

        public ActorController(IRepositry<Actor> actorRepositry)
        {
            _actorRepositry = actorRepositry;
        }

        public async Task<IActionResult> Index()
        {
            //return View(_context.Actors.ToList());
            return View((await _actorRepositry.GetListAsync()).ToList());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var actors = _context.Actors.AsQueryable();
            var actors = await _actorRepositry.GetListAsync();
            return View(actors.AsEnumerable());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Actor actor , IFormFile ImageFile)
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
            //_context.Actors.Add(actor);
            await _actorRepositry.AddAsync(actor);
            //_context.SaveChanges();
            await _actorRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var actor = _context.Actors.FirstOrDefault(c => c.Id == id);
            var actor = await _actorRepositry.GetOneAsync(c => c.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Actor actor , IFormFile ImageFile)
        {
            //var actorInDb = _context.Actors.AsNoTracking().FirstOrDefault(b => b.Id == actor.Id);
            var actorInDb = await _actorRepositry.GetOneAsync(b => b.Id == actor.Id, isTracked: false);
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
            //_context.Actors.Update(actor);
            _actorRepositry.Update(actor);
            //_context.SaveChanges();
            await _actorRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var actor = _context.Actors.FirstOrDefault(c => c.Id == id);
            var actor = await _actorRepositry.GetOneAsync(c => c.Id == id);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Actor", actor.Img);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            //_context.Actors.Remove(actor);
            _actorRepositry.Remove(actor);
            //_context.SaveChanges();
            await _actorRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
