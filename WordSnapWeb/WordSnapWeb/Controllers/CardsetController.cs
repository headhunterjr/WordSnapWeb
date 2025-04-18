using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Runtime.InteropServices;
using WordSnapWeb.Models;

namespace WordSnapWeb.Controllers
{
    [Route("Cardset")]
    public class CardsetController : Controller
    {
        private readonly IWordSnapRepository _repository;
        private readonly UserManager<ApplicationUser> _users;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CardsetController(IWordSnapRepository repository, UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signInManager)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [HttpGet("{cardsetId}")]
        public async Task<IActionResult> Details(int cardsetId)
        {
            var cardset = await _repository.GetCardsetByIdAsync(cardsetId);
            if (cardset == null)
            {
                return NotFound();
            }
            if (_signInManager.IsSignedIn(User))
            {
                var userscardset = await _repository.GetUserscardsetAsync(_users.GetUserId(User), cardsetId);
                ViewBag.IsSaved = (userscardset != null);
            }
            else
            {
                ViewBag.IsSaved = false;
            }

            return View(cardset);
        }

        [Authorize]
        [HttpPost("{cardsetId}/AddCard")]
        public async Task<IActionResult> CreateCard(int cardsetId, Card card)
        {
            card.CardsetRef = cardsetId;
            var cardset = await _repository.GetCardsetByIdAsync(cardsetId);
            var isAdmin = User.IsInRole("Admin");
            var userId = _users.GetUserId(User);

            if (cardset.UserRef == userId || isAdmin)
            {
                await _repository.AddCardAsync(card);
                return RedirectToAction("Details", new { cardsetId });
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpPost("AddToLibrary/{cardsetId}")]
        public async Task<IActionResult> AddToLibrary(int cardsetId)
        {
            var existing = await _repository.GetUserscardsetAsync(_users.GetUserId(User), cardsetId);
            if (existing == null)
            {
                var userscardset = new Userscardset
                {
                    UserRef = _users.GetUserId(User),
                    CardsetRef = cardsetId
                };
                await _repository.AddCardsetToSavedLibraryAsync(userscardset);
            }
            return RedirectToAction("Details", new { cardsetId });
        }

        [Authorize]
        [HttpPost("RemoveFromLibrary/{cardsetId}")]
        public async Task<IActionResult> RemoveFromLibrary(int cardsetId)
        {
            var existing = await _repository.GetUserscardsetAsync(_users.GetUserId(User), cardsetId);
            if (existing != null)
            {
                await _repository.DeleteUsersCardset(existing.Id);
            }
            return RedirectToAction("Details", new { cardsetId });
        }

        [Authorize]
        [HttpPost("EditCardset")]
        public async Task<IActionResult> EditCardset(Cardset updatedCardset)
        {
            var cardset = await _repository.GetCardsetByIdAsync(updatedCardset.Id);
            if (cardset == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole("Admin");
            var userId = _users.GetUserId(User);

            if (cardset.UserRef == userId || isAdmin)
            {
                cardset.Name = updatedCardset.Name;
                cardset.IsPublic = updatedCardset.IsPublic;

                await _repository.UpdateCardsetAsync(cardset);

                return RedirectToAction("Details", new { cardsetId = cardset.Id });
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpPost("DeleteCardset")]
        public async Task<IActionResult> DeleteCardset(int cardsetId)
        {
            var cardset = await _repository.GetCardsetByIdAsync(cardsetId);
            var isAdmin = User.IsInRole("Admin");
            var userId = _users.GetUserId(User);

            if (cardset.UserRef == userId || isAdmin)
            {
                await _repository.DeleteCardsetAsync(cardsetId);
                return RedirectToAction("Index", "Home");
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("{cardsetId}/TakeTest")]
        public async Task<IActionResult> TakeTest(int cardsetId)
        {
            var cardset = await _repository.GetCardsetByIdAsync(cardsetId);
            if (cardset == null)
                return NotFound();

            if (!cardset.Cards.Any())
            {
                TempData["Error"] = "Немає карток для тесту.";
                return RedirectToAction("Details", new { cardsetId });
            }
            var progress = await _repository.GetProgress(_users.GetUserId(User), cardsetId);
            double bestScore = 0;
            if (progress != null)
            {
                bestScore = progress.SuccessRate ?? 0;
            }
            var model = new TestViewModel
            {
                CardsetId = cardset.Id,
                CardsetName = cardset.Name,
                Cards = cardset.Cards.ToList(),
                BestScore = bestScore
            };

            return View(model);
        }

        [Authorize]
        [HttpPost("{cardsetId}/TakeTest")]
        public async Task<IActionResult> TakeTest(int cardsetId, double score)
        {
            var progress = await _repository.GetProgress(_users.GetUserId(User), cardsetId);
            if (progress == null)
            {
                var newProgress = new Progress
                {
                    UserRef = _users.GetUserId(User),
                    CardsetRef = cardsetId,
                    SuccessRate = score,
                    LastAccessed = DateTime.Now
                };
                await _repository.AddTestProgressAsync(newProgress);
            }
            else if (score > progress.SuccessRate)
            {
                progress.SuccessRate = score;
                progress.LastAccessed = DateTime.Now;
                await _repository.UpdateProgress(progress);
            }
            return RedirectToAction("Details", new { cardsetId });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
