using Microsoft.AspNetCore.Mvc;
using WordSnapWeb.Models;

namespace WordSnapWeb.Controllers
{
    [Route("User/{username}")]
    public class UserController : Controller
    {
        private readonly IWordSnapRepository _repository;
        // test user id and username
        private const int CurrentUserId = 82;
        private const string CurrentUsername = "test_username";

        public UserController(IWordSnapRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("SavedLibrary")]
        public async Task<IActionResult> SavedLibrary(string username)
        {
            if (username != CurrentUsername)
            {
                return NotFound();
            }

            var cardsets = await _repository.GetUsersCardsetsLibraryAsync(CurrentUserId);
            ViewBag.Username = username;
            return View(cardsets);
        }
    }
}