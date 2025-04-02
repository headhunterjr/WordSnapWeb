using Microsoft.AspNetCore.Mvc;
using WordSnapWeb.Models;
using WordSnapWeb.Services;

namespace WordSnapWeb.Controllers
{
    [Route("User/{username}")]
    public class UserController : Controller
    {
        private readonly IWordSnapRepository _repository;

        public UserController(IWordSnapRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("SavedLibrary")]
        public async Task<IActionResult> SavedLibrary(string username)
        {
            if (!UserSession.Instance.IsLoggedIn)
            {
                return RedirectToAction("Login", "Account");
            }
            var currentUser = UserSession.Instance.CurrentUser;
            if (username != currentUser.Username)
            {
                return NotFound();
            }

            var cardsets = await _repository.GetUsersCardsetsLibraryAsync(currentUser.Id);
            ViewBag.Username = username;
            return View(cardsets);
        }
    }
}
