using BellosoftWebApi.DeckOfCardsApi;

namespace BellosoftWebApi.Services
{
    public interface IDeckOfCards
    {
        public Task<ApiDeckCreateResponse?> CreateDeck(int amount);
        public Task<ApiDeckDrawResponse?> DrawCard(string id, int amount);
    }
}
