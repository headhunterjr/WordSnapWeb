using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WordSnapWeb.Models;

namespace WordSnapWeb.Controllers
{
    public class CardController : Controller
    {
        private readonly IWordSnapRepository _repository;
        private readonly UserManager<ApplicationUser> _users;

        public CardController(IWordSnapRepository repository, UserManager<ApplicationUser> users)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _users = users ?? throw new ArgumentNullException(nameof(users));
        }

        [Authorize]
        [HttpGet("EditCard/{cardId}")]
        public async Task<IActionResult> EditCard(int cardId)
        {
            var card = await _repository.GetCardAsync(cardId);
            if (card == null)
            {
                return NotFound();
            }
            var isAdmin = User.IsInRole("Admin");
            var userId = _users.GetUserId(User);
            var cardset = await _repository.GetCardsetByIdAsync(card.CardsetRef);
            if (cardset.UserRef == userId || isAdmin)
            {
                return View(card);
            }
            return Unauthorized();
        }

        [Authorize]
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

            var isAdmin = User.IsInRole("Admin");
            var userId = _users.GetUserId(User);
            var cardset = await _repository.GetCardsetByIdAsync(card.CardsetRef);
            if (cardset.UserRef == userId || isAdmin)
            {
                card.WordEn = updatedCard.WordEn;
                card.WordUa = updatedCard.WordUa;
                card.Comment = updatedCard.Comment;

                await _repository.UpdateCardAsync(card);

                return RedirectToAction("Details", "Cardset", new { cardsetId = card.CardsetRef });
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpPost("DeleteCard/{cardId}")]
        public async Task<IActionResult> DeleteCard(int cardId)
        {
            var card = await _repository.GetCardAsync(cardId);
            if (card == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole("Admin");
            var userId = _users.GetUserId(User);
            var cardset = await _repository.GetCardsetByIdAsync(card.CardsetRef);
            if (cardset.UserRef == userId || isAdmin)
            {
                int cardsetId = card.CardsetRef;
                await _repository.DeleteCardAsync(cardId);
                return RedirectToAction("Details", "Cardset", new { cardsetId });
            }
            return Unauthorized();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
