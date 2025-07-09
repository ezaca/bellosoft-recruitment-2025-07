using BellosoftWebApi.Services.Sqids;

namespace BellosoftWebApi.Responses
{
    public class DeckCreateResponse
    {
        public string DeckId { get; set; }
        public int RemainingCards { get; set; }

        public DeckCreateResponse(ISqidsGenerator sqids, int deckId, int remainingCards)
            : this(sqids.Encode(deckId), remainingCards)
        { }

        public DeckCreateResponse(string deckHashId, int remainingCards)
        {
            DeckId = deckHashId;
            RemainingCards = remainingCards;
        }
    }
}
