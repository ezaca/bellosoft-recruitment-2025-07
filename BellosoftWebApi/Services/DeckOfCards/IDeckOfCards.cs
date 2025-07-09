using BellosoftWebApi.DeckOfCardsApi;

namespace BellosoftWebApi.Services
{
    public interface IDeckOfCards
    {
        public Task<DeckCreateApiResponse> CreateDeck(int amount);
        public Task<DeckDrawResponse> DrawCard(string id, int amount);
    }
}
