using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _singInManger;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> singInManger)
        {
            _userManager = userManager;
            _singInManger = singInManger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            ApplicationUser user = new ApplicationUser()
            {
                Name = registerVM.Name,
                UserName = registerVM.UserName,
                Email = registerVM.Email
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerVM);
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);

            if(user == null)
            {
                ModelState.AddModelError("", "invalid userName or Email or Password");
                return View(loginVM);
            }

            var result = await _singInManger.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "You are Locked please try again later");
                }
                else
                {
                    ModelState.AddModelError("", "invalid userName or Email or Password");
                }
                return View(loginVM);
            }
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
    }
}
