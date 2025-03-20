
using Microsoft.EntityFrameworkCore;

namespace WordSnapWeb.Models
{
    public class WordSnapRepository : IWordSnapRepository
    {
        private readonly WordSnapDbContext _context;

        public WordSnapRepository(WordSnapDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> AddCardAsync(Card card)
        {
            await _context.Cards.AddAsync(card);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> AddCardsetAsync(Cardset cardset)
        {
            await _context.Cardsets.AddAsync(cardset);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteCardAsync(int cardId)
        {
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == cardId);
            if (card == null)
            {
                throw new InvalidOperationException("Картку не знайдено.");
            }

            _context.Cards.Remove(card);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteCardsetAsync(int cardsetId)
        {
            var cardset = await _context.Cardsets.FirstOrDefaultAsync(cs => cs.Id == cardsetId);
            if (cardset == null)
            {
                throw new InvalidOperationException("Набір карток не знайдено");
            }

            _context.Cardsets.Remove(cardset);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateCardAsync(Card card)
        {
            _context.Cards.Update(card);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateCardsetAsync(Cardset cardset)
        {
            _context.Cardsets.Update(cardset);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SwitchCardsetPrivacyAsync(int cardsetId)
        {
            var cardset = await _context.Cardsets.FirstOrDefaultAsync(cs => cs.Id == cardsetId);
            if (cardset == null)
            {
                throw new InvalidOperationException("Набір карток не знайдено");
            }

            cardset.IsPublic = !cardset.IsPublic;
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Cardset>> GetCardsetsFromSearchAsync(string searchQuery)
        {
            var cardsets = await _context.Cardsets.Where(cs => cs.Name.ToLower().Contains(searchQuery.ToLower())).Where(cs => cs.IsPublic ?? false).ToListAsync();
            return cardsets;
        }

        public async Task<Cardset> GetCardsetByIdAsync(int id)
        {
            var cardset = await _context.Cardsets.Include(cs => cs.Cards).FirstOrDefaultAsync(cs => cs.Id == id);
            return cardset ?? throw new InvalidOperationException("Набір карток не знайдено");
        }

        public async Task<Card?> GetCardAsync(int cardId)
        {
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == cardId);
            return card;
        }
    }
}
