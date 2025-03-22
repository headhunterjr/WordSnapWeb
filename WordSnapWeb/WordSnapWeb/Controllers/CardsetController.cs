using Microsoft.AspNetCore.Mvc;
using WordSnapWeb.Models;

namespace WordSnapWeb.Controllers
{
    [Route("Cardset")]
    public class CardsetController : Controller
    {
        private readonly IWordSnapRepository _repository;

        // test user id
        private const int CurrentUserId = 82;

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
            var userscardset = await _repository.GetUserscardsetAsync(CurrentUserId, cardsetId);
            ViewBag.IsSaved = (userscardset != null);

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
            var existing = await _repository.GetUserscardsetAsync(CurrentUserId, cardsetId);
            if (existing == null)
            {
                var userscardset = new Userscardset
                {
                    UserRef = CurrentUserId,
                    CardsetRef = cardsetId
                };
                await _repository.AddCardsetToSavedLibraryAsync(userscardset);
            }
            return RedirectToAction("Details", new { cardsetId });
        }

        [HttpPost("RemoveFromLibrary/{cardsetId}")]
        public async Task<IActionResult> RemoveFromLibrary(int cardsetId)
        {
            var existing = await _repository.GetUserscardsetAsync(CurrentUserId, cardsetId);
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
            var progress = await _repository.GetProgress(CurrentUserId, cardsetId);
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
            var progress = await _repository.GetProgress(CurrentUserId, cardsetId);
            if (progress == null)
            {
                var newProgress = new Progress
                {
                    UserRef = CurrentUserId,
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
