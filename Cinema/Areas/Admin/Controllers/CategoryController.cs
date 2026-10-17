using Cinema.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            return View(_context.Categories.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.AsQueryable();
            return View(categories.AsEnumerable());
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
