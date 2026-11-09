using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using Cinema.Repositories;
namespace Cinema.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _singInManger;
        private readonly IEmailSender _emailSender;
        private readonly IRepositry<ApplicationUserOtp> _applicationUserOtpRepository;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> singInManger, IEmailSender emailSender, IRepositry<ApplicationUserOtp> applicationUserOtpRepository)
        {
            _userManager = userManager;
            _singInManger = singInManger;
            _emailSender = emailSender;
            _applicationUserOtpRepository = applicationUserOtpRepository;
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
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", id = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                registerVM.Email,
                "Cinema confirmation email",
                $"<h1>Please click <a href = '{link}'> here</a> to confirm your mail</h1>"
                );

            TempData["Successfull_Notification"] = "Registration Successfully";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string id, string token)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }
            return RedirectToAction(nameof(Login));
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

            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var result = await _singInManger.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "You are Locked please try again later");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Please confirm your email first");
                    ViewBag.ShowResend = true;                              // <-- هنا
                    ViewBag.UserNameOrEmail = loginVM.UserNameOrEmail;
                }
                else
                {
                    ModelState.AddModelError("", "invalid userName or Email or Password");
                }
                return View(loginVM);
            }
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid ||string.IsNullOrWhiteSpace(resendEmailConfirmationVM.UserNameOrEmail))
            {
                ModelState.AddModelError("", "Please enter your username or email.");
                return View(resendEmailConfirmationVM);
            }

            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail) ??
                        await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "invalid userName or Email or Password");
                return View(resendEmailConfirmationVM);
            }
            if (user.EmailConfirmed)
            {
                ModelState.AddModelError("", "email already confirmed");
                return View(resendEmailConfirmationVM);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", id = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                user.Email,
                "Cinema Confiramtion Email",
                $"<h1> please click <a href='{link}'> here </a>to confirm Your Mail</h1>"
                );

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "invalid userName or Email or Password");
                return View(forgetPasswordVM);
            }

            var otp = new Random().Next(1000, 9999).ToString();
            var applicationUserOtp = new ApplicationUserOtp(otp, user.Id);

            await _applicationUserOtpRepository.AddAsync(applicationUserOtp);
            await _applicationUserOtpRepository.CommitAsync();

            await _emailSender.SendEmailAsync(
                user.Email,
                "ECommerce533 Reset Email",
                $"<h1>Use this {otp} to reset tour password</h1>"
                );
            return RedirectToAction(nameof(VerifyOtp), new { userId = user.Id });
        }

        [HttpGet]
        public IActionResult VerifyOtp(string userId)
        {
            return View(new VerifyOtpVM() { UserId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpVM verifyOtpVM)
        {
            var user = await _userManager.FindByIdAsync(verifyOtpVM.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid User");
                return View(verifyOtpVM);
            }

            var applicationUserOtps = await _applicationUserOtpRepository.GetListAsync(a =>
                a.ApplicationUserId == user.Id &&
                a.IsValid &&
                a.ValidTo >= DateTime.UtcNow
            );

            var applicationUserOtp = applicationUserOtps.OrderBy(a => a.CreatedAt).LastOrDefault();

            if (applicationUserOtp == null || applicationUserOtp.OTP != verifyOtpVM.OTP)
            {
                ModelState.AddModelError("", "invalid OTP or Expired");
                return View(verifyOtpVM);
            }
            applicationUserOtp.IsValid = false;
            await _applicationUserOtpRepository.CommitAsync();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return RedirectToAction(nameof(ResetPassword), new { userId = user.Id, token });
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPasswordVM() { UserId = userId, Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordVM);
            }
            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid User");
                return View(resetPasswordVM);
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(resetPasswordVM);
            }
            return RedirectToAction(nameof(Login));
        }
    }
}
