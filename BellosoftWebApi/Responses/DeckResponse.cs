using BellosoftWebApi.Services.Sqids;

namespace BellosoftWebApi.Responses
{
    public class DeckResponse
    {
        public string DeckId { get; set; }
        public int RemainingCards { get; set; }

        public DeckResponse(string deckId, int remainingCards)
        {
            DeckId = deckId;
            RemainingCards = remainingCards;
        }

        public DeckResponse(ISqidsGenerator sqids, int deckDatabaseId, int remainingCards)
            : this(sqids.Encode(deckDatabaseId), remainingCards) { }
    }
}
