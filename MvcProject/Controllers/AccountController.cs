using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Models;
using System.Application.Abstraction;

namespace MvcProject.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IStoreService _storeService;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<AccountController> logger,
            IStoreService storeService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _storeService = storeService;
        }

        private void CheckForErrorMessage()
        {
            if (HttpContext.Items.ContainsKey("ErrorMessage"))
            {
                TempData["ErrorMessage"] = HttpContext.Items["ErrorMessage"]?.ToString();
            }
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CheckForErrorMessage();
                return View(model);
            }

            try
            {
                // Check if the email is associated with a store
                var store = await _storeService.GetStoreByOwnerEmailAsync(model.Email);
                if (store == null)
                {
                    _logger.LogWarning("Registration attempt with email {Email} failed: No store associated.", model.Email);
                    ModelState.AddModelError(string.Empty, "This email is not associated with any store. Please contact the admin.");
                    CheckForErrorMessage();
                    return View(model);
                }

                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} registered successfully.", model.Email);
                    await _userManager.AddToRoleAsync(user, "Owner");
                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                _logger.LogWarning("Registration attempt for user {Email} failed.", model.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user {Email}.", model.Email);
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            CheckForErrorMessage();
            return View(model);
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