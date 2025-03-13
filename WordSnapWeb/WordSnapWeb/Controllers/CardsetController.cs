using Microsoft.AspNetCore.Mvc;
using WordSnapWeb.Models;

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
            return View(cardset);
        }

        [HttpPost("{cardsetId}/AddCard")]
        public async Task<IActionResult> CreateCard(int cardsetId, Card card)
        {
            card.CardsetRef = cardsetId;
            await _repository.AddCardAsync(card);
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


        public IActionResult Index()
        {
            return View();
        }
    }
}
