using BellosoftWebApi.Services.Sqids;

namespace BellosoftWebApi.Responses
{
    public class DeckListResponse
    {
        public string? SelectedDeck { get; set; }
        public List<DeckResponse> Decks { get; set; }

        public DeckListResponse(string? selectedDeck, List<DeckResponse> decks)
        {
            SelectedDeck = selectedDeck;
            Decks = decks;
        }

        public DeckListResponse(ISqidsGenerator sqids, int? deckDatabaseId, List<DeckResponse> decks)
            : this(deckDatabaseId is null ? null : sqids.Encode((int)deckDatabaseId), decks) { }
    }
}
