using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Models;

namespace MvcProject.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        private void CheckForErrorMessage()
        {
            if (HttpContext.Items.ContainsKey("ErrorMessage"))
            {
                TempData["ErrorMessage"] = HttpContext.Items["ErrorMessage"]?.ToString();
            }
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} logged in successfully.", model.Email);
                    string returnUrl = model.ReturnUrl ?? Url.Action("Index", "Home")!;
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {Email} is locked out due to multiple failed login attempts.", model.Email);
                    ModelState.AddModelError(string.Empty, "This account has been locked out. Please try again later.");
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for user {Email}.", model.Email);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in user {Email}.", model.Email);
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            CheckForErrorMessage();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userEmail = User.Identity?.Name;
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User {Email} logged out successfully.", userEmail ?? "Unknown");
            return RedirectToAction("Index", "Home");
        }
    }

  
}