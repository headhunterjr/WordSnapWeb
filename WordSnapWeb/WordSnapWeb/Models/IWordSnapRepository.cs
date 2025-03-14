﻿namespace WordSnapWeb.Models
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
        public Task<int> DeleteCardFromCardset(int cardId);
        public Task<Card?> GetCardAsync(int cardId);
    }
}
