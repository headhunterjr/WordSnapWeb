using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using WordSnapWeb.Models;
using WordSnapWeb.Services;

namespace WordSnapWeb.Controllers
{
    [Route("Cardset")]
    public class CardsetController : Controller
    {
        private readonly IWordSnapRepository _repository;


        public CardsetController(IWordSnapRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{cardsetId}")]
        public async Task<IActionResult> Details(int cardsetId)
        {
            var cardset = await _repository.GetCardsetByIdAsync(cardsetId);
            if (cardset == null)
            {
                return NotFound();
            }
            if (UserSession.Instance.IsLoggedIn)
            {
                var userscardset = await _repository.GetUserscardsetAsync(UserSession.Instance.CurrentUser.Id, cardsetId);
                ViewBag.IsSaved = (userscardset != null);
            }
            else
            {
                ViewBag.IsSaved = false;
            }

            return View(cardset);
        }

        [HttpPost("{cardsetId}/AddCard")]
        public async Task<IActionResult> CreateCard(int cardsetId, Card card)
        {
            card.CardsetRef = cardsetId;
            await _repository.AddCardAsync(card);
            return RedirectToAction("Details", new { cardsetId });
        }

        [HttpPost("AddToLibrary/{cardsetId}")]
        public async Task<IActionResult> AddToLibrary(int cardsetId)
        {
            var existing = await _repository.GetUserscardsetAsync(UserSession.Instance.CurrentUser.Id, cardsetId);
            if (existing == null)
            {
                var userscardset = new Userscardset
                {
                    UserRef = UserSession.Instance.CurrentUser.Id,
                    CardsetRef = cardsetId
                };
                await _repository.AddCardsetToSavedLibraryAsync(userscardset);
            }
            return RedirectToAction("Details", new { cardsetId });
        }

        [HttpPost("RemoveFromLibrary/{cardsetId}")]
        public async Task<IActionResult> RemoveFromLibrary(int cardsetId)
        {
            var existing = await _repository.GetUserscardsetAsync(UserSession.Instance.CurrentUser.Id, cardsetId);
            if (existing != null)
            {
                await _repository.DeleteUsersCardset(existing.Id);
            }
            return RedirectToAction("Details", new { cardsetId });
        }

        [HttpPost("EditCardset")]
        public async Task<IActionResult> EditCardset(Cardset updatedCardset)
        {
            var cardset = await _repository.GetCardsetByIdAsync(updatedCardset.Id);
            if (cardset == null)
            {
                return NotFound();
            }

            cardset.Name = updatedCardset.Name;
            cardset.IsPublic = updatedCardset.IsPublic;

            await _repository.UpdateCardsetAsync(cardset);

            return RedirectToAction("Details", new { cardsetId = cardset.Id });
        }

        [HttpPost("DeleteCardset")]
        public async Task<IActionResult> DeleteCardset(int cardsetId)
        {
            await _repository.DeleteCardsetAsync(cardsetId);
            return RedirectToAction("Index", "Home");
        }

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
            var progress = await _repository.GetProgress(UserSession.Instance.CurrentUser.Id, cardsetId);
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

        [HttpPost("{cardsetId}/TakeTest")]
        public async Task<IActionResult> TakeTest(int cardsetId, double score)
        {
            var progress = await _repository.GetProgress(UserSession.Instance.CurrentUser.Id, cardsetId);
            if (progress == null)
            {
                var newProgress = new Progress
                {
                    UserRef = UserSession.Instance.CurrentUser.Id,
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
