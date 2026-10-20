using Cinema.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(string Name , int page = 1)
        {
            var categories = _context.Categories.AsQueryable();

            // filter 
            if(Name != null)
            {
                categories = categories.Where(a => a.Name.Contains(Name));
            }

            // pagination
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(categories.Count() / 4.0);
            ViewBag.CurrentPage = page;

            categories = categories.Skip((page - 1) * 4).Take(4);
            return View(categories.ToList());
        }
    }
}
