using Cinema.Data;
using Cinema.Models;
using Cinema.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepositry<Category> _categoryRepositry;// = new Repositry<Category>();

        public CategoryController(IRepositry<Category> categoryRepositry)
        {
            _categoryRepositry = categoryRepositry;
        }

        public async Task<IActionResult> Index()
        {
            //return View(_context.Categories.ToList());
            return View((await _categoryRepositry.GetListAsync()).ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            //_context.Categories.Add(category);
            await _categoryRepositry.AddAsync(category);
            //_context.SaveChanges();
            await _categoryRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = await _categoryRepositry.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            //_context.Categories.Update(category);
            _categoryRepositry.Update(category);
            //_context.SaveChanges();
            await _categoryRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = await _categoryRepositry.GetOneAsync(c => c.Id == id);
            //_context.Categories.Remove(category);
            _categoryRepositry.Remove(category);
            //_context.SaveChanges();
            await _categoryRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
