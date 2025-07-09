using BellosoftWebApi.DeckOfCardsApi;
using BellosoftWebApi.Services.Sqids;

namespace BellosoftWebApi.Responses
{
    public class DrawCardResponse
    {
        public string DeckId { get; set; }
        public int RemainingCards { get; set; }
        public List<CardData> Cards { get; set; }

        public DrawCardResponse(string deckId, int remainingCards, List<CardData> card)
        {
            DeckId = deckId;
            RemainingCards = remainingCards;
            Cards = card;
        }

        public DrawCardResponse(ISqidsGenerator sqids, int deckDatabaseId, int remainingCards, List<CardData> card)
            : this(sqids.Encode(deckDatabaseId), remainingCards, card) { }
    }
}
