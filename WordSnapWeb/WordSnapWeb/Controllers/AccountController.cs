using Microsoft.AspNetCore.Mvc;
using WordSnapWeb.Services;
using WordSnapWeb.Models;

namespace WordSnapWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthenticationService _authService;

        public AccountController(AuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (UserSession.Instance.IsLoggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (success, message, user) = await _authService.LoginAsync(model.Email, model.Password);

                if (success)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, message);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (UserSession.Instance.IsLoggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (success, message, user) = await _authService.RegisterAsync(model.Username, model.Email, model.Password);

                if (success)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, message);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _authService.Logout();
            return RedirectToAction("Login");
        }
    }
}
