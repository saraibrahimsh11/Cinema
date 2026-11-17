using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
namespace Cinema.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManger;

        public ProfileController(UserManager<ApplicationUser> userManger)
        {
            _userManger = userManger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManger.GetUserAsync(User);
            var userVM = user.Adapt<ApplicationUserVM>();
            return View(userVM);
        }

        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManger.GetUserAsync(User);
            user.Name = applicationUserVM.Name;
            user.PhoneNumber = applicationUserVM.PhoneNumber;

            var result = await _userManger.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                TempData["Error_Notification"] = errors;
            }
            else
            {
                TempData["Successful_Notification"] = "update information Successfully";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            if (string.IsNullOrEmpty(applicationUserVM.CurrentPassword))
            {
                TempData["Error_Notification"] = "Current password is required";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManger.GetUserAsync(User);
            
            var result = await _userManger.ChangePasswordAsync(user, applicationUserVM.CurrentPassword, applicationUserVM.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                TempData["Error_Notification"] = errors;
            }
            else
            {
                TempData["Successful_Notification"] = "update password Successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
