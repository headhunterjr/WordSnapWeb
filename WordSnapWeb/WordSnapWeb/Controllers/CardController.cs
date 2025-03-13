using Microsoft.AspNetCore.Mvc;
using WordSnapWeb.Models;

namespace WordSnapWeb.Controllers
{
    public class CardController : Controller
    {
        private readonly IWordSnapRepository _repository;

        public CardController(IWordSnapRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("EditCard/{cardId}")]
        public async Task<IActionResult> EditCard(int cardId)
        {
            var card = await _repository.GetCardAsync(cardId);
            if (card == null)
            {
                return NotFound();
            }

            return View(card);
        }

        [HttpPost("EditCard/{cardId}")]
        public async Task<IActionResult> EditCard(int cardId, Card updatedCard)
        {
            if (cardId != updatedCard.Id)
            {
                return BadRequest();
            }

            var card = await _repository.GetCardAsync(cardId);
            if (card == null)
            {
                return NotFound();
            }

            card.WordEn = updatedCard.WordEn;
            card.WordUa = updatedCard.WordUa;
            card.Comment = updatedCard.Comment;

            await _repository.UpdateCardAsync(card);

            return RedirectToAction("Details", "Cardset", new { cardsetId = card.CardsetRef });
        }

        [HttpPost("DeleteCard/{cardId}")]
        public async Task<IActionResult> DeleteCard(int cardId)
        {
            var card = await _repository.GetCardAsync(cardId);
            if (card == null)
            {
                return NotFound();
            }

            int cardsetId = card.CardsetRef;

            await _repository.DeleteCardAsync(cardId);

            return RedirectToAction("Details", "Cardset", new { cardsetId });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
