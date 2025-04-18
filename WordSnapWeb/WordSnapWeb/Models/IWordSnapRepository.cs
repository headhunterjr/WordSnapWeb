namespace WordSnapWeb.Models
{
    public interface IWordSnapRepository
    {
        public Task<int> AddCardsetAsync(Cardset cardset);
        public Task<int> AddCardAsync(Card card);
        public Task<IEnumerable<Cardset>> GetCardsetsFromSearchAsync(string searchQuery);
        public Task<Cardset> GetCardsetByIdAsync(int id);
        public Task<int> SwitchCardsetPrivacyAsync(int cardsetId);
        public Task<int> DeleteCardAsync(int cardId);
        public Task<int> DeleteCardsetAsync(int cardsetId);
        public Task<int> UpdateCardAsync(Card card);
        public Task<int> UpdateCardsetAsync(Cardset cardset);
        public Task<Card?> GetCardAsync(int cardId);
        public Task<int> AddCardsetToSavedLibraryAsync(Userscardset userscardset);
        public Task<IEnumerable<Cardset>> GetUsersCardsetsLibraryAsync(string userId);
        public Task<Userscardset?> GetUserscardsetAsync(string userId, int cardsetId);
        public Task<bool> DeleteUsersCardset(int userscardsetId);
        public Task<int> AddTestProgressAsync(Progress progress);
        public Task<Progress?> GetProgress(string userId, int cardsetId);
        public Task<int> UpdateProgress(Progress progress);
    }
}
