using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WordSnapWeb.Models;

namespace WordSnapWeb.Controllers
{
    [Route("User/{username}")]
    public class UserController : Controller
    {
        private readonly IWordSnapRepository _repository;
        private readonly UserManager<ApplicationUser> _users;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(IWordSnapRepository repository, UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signInManager = null)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [Authorize]
        [HttpGet("SavedLibrary")]
        public async Task<IActionResult> SavedLibrary(string username)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Login", "Account");
            }
            var currentUser = _users.GetUserName(User);
            if (username != currentUser)
            {
                return NotFound();
            }

            var cardsets = await _repository.GetUsersCardsetsLibraryAsync(_users.GetUserId(User));
            ViewBag.Username = username;
            return View(cardsets);
        }
    }
}
